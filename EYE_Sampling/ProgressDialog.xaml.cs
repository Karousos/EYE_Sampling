using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace EYE_Sampling
{
  /// <summary>
  /// A simple progress dialog that invokes clients via
  /// a synchronous event which is called on a worker thread.
  /// </summary>
  /// <example>
  /// This example creates a new dialog instance, registers an
  /// event handler for the worker thread, and displays the dialog
  /// by invoking the <see cref="RunWorkerThread(DoWorkEventHandler)"/>
  /// or <see cref="RunWorkerThread(object,DoWorkEventHandler)"/> methods:
  ///  <code>
  /// //declare background worker method
  /// DoWorkEventHandler handler = delegate
  /// {
  ///   SaveProject();
  /// }
  /// 
  /// //init progress dialog
  /// ProgressDialog dlg = new ProgressDialog("Saving project...");
  /// dlg.AutoIncrementInterval = 200;
  /// dlg.Owner = Application.Current.MainWindow;
  /// 
  /// //run work
  /// dlg.RunWorkerThread(handler);
  /// 
  /// if (dlg.Error != null)
  /// {
  ///   Console.Out.Writeline("An error occurred: " + dlg.Error.Message);
  /// }
  /// </code>
  /// </example>
  public partial class ProgressDialog : Window
  {
    #region fields

    /// <summary>
    /// The background worker which handles asynchronous invocation
    /// of the worker method.
    /// </summary>
    private readonly BackgroundWorker worker;

    /// <summary>
    /// The timer to be used for automatic progress bar updated.
    /// </summary>
    private readonly DispatcherTimer progressTimer;

    /// <summary>
    /// The UI culture of the thread that invokes the dialog.
    /// </summary>
    private CultureInfo uiCulture;

    /// <summary>
    /// If set, the interval in which the progress bar
    /// gets incremented automatically.
    /// </summary>
    private int? autoIncrementInterval = null;

    /// <summary>
    /// Whether background processing was cancelled by the user.
    /// </summary>
    private bool cancelled = false;

    /// <summary>
    /// Defines the size of a single increment of the progress bar.
    /// Defaults to 5.
    /// </summary>
    private int progressBarIncrement = 5;

    /// <summary>
    /// Provides an exception that occurred during the asynchronous
    /// operation on the worker thread. Defaults to null, which
    /// indicates that no exception occurred at all.
    /// </summary>
    private Exception error = null;

    /// <summary>
    /// The result, if assigned to the <see cref="DoWorkEventArgs.Result"/>
    /// property by the worker method.
    /// </summary>
    private object result = null;

    /// <summary>
    /// The 
    /// </summary>
    private DoWorkEventHandler workerCallback;

    #endregion


    #region properties

    /// <summary>
    /// Gets or sets the dialog text.
    /// </summary>
    public string DialogText
    {
      get { return txtDialogMessage.Text; }
      set { txtDialogMessage.Text = value; }
    }


    

    /// <summary>
    /// Whether the process was cancelled by the user.
    /// </summary>
    public bool Cancelled
    {
      get { return cancelled; }
    }

    /// <summary>
    /// If set, the interval in which the progress bar
    /// gets incremented automatically.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">If the interval
    /// is lower than 100 ms.</exception>
    public int? AutoIncrementInterval
    {
      get { return autoIncrementInterval; }
      set
      {
        if (value.HasValue && value < 100) throw new ArgumentOutOfRangeException("value");
        autoIncrementInterval = value;
      }
    }

    /// <summary>
    /// Defines the size of a single increment of the progress bar.
    /// The default value is 5, with a progress bar range of 0 - 100.
    /// </summary>
    public int ProgressBarIncrement
    {
      get { return progressBarIncrement; }
      set { progressBarIncrement = value; }
    }

    /// <summary>
    /// Provides an exception that occurred during the asynchronous
    /// operation on the worker thread. Defaults to null, which
    /// indicates that no exception occurred at all.
    /// </summary>
    public Exception Error
    {
      get { return error; }
    }

    /// <summary>
    /// The result, if assigned to the <see cref="DoWorkEventArgs.Result"/>
    /// property by the worker method. Defaults to null.
    /// </summary>
    public object Result
    {
      get { return result; }
    }


    /// <summary>
    /// Shows or hides the progressbar control. Defaults to
    /// true.
    /// </summary>
    public bool ShowProgressBar
    {
      get { return progressBar.Visibility == Visibility.Visible; }
      set { progressBar.Visibility = value ? Visibility.Visible : Visibility.Hidden; }
    }

    #endregion


    /// <summary>
    /// Inits the dialog with a given dialog text.
    /// </summary>
    public ProgressDialog(string dialogText) : this()
    {
      DialogText = dialogText;
    }


    /// <summary>
    /// Inits the dialog without displaying it.
    /// </summary>
    public ProgressDialog()
    {
      InitializeComponent();

      //init the timer
      progressTimer = new DispatcherTimer(DispatcherPriority.SystemIdle, Dispatcher);
      progressTimer.Tick += OnProgressTimer_Tick;

      //init background worker
      worker = new BackgroundWorker();
      worker.WorkerReportsProgress = true;
      worker.WorkerSupportsCancellation = true;

      worker.DoWork += worker_DoWork;
      worker.ProgressChanged += worker_ProgressChanged;
      worker.RunWorkerCompleted += worker_RunWorkerCompleted;
    }


    #region run worker thread

    /// <summary>
    /// Launches a worker thread which is intendet to perform
    /// work while progress is indicated.
    /// </summary>
    /// <param name="workHandler">A callback method which is
    /// being invoked on a background thread in order to perform
    /// the work to be performed.</param>
    public bool RunWorkerThread(DoWorkEventHandler workHandler)
    {
      return RunWorkerThread(null, workHandler);
    }


    /// <summary>
    /// Launches a worker thread which is intended to perform
    /// work while progress is indicated, and displays the dialog
    /// modally in order to block the calling thread.
    /// </summary>
    /// <param name="argument">A custom object which will be
    /// submitted in the <see cref="DoWorkEventArgs.Argument"/>
    /// property <paramref name="workHandler"/> callback method.</param>
    /// <param name="workHandler">A callback method which is
    /// being invoked on a background thread in order to perform
    /// the work to be performed.</param>
    public bool RunWorkerThread(object argument, DoWorkEventHandler workHandler)
    {
      if (autoIncrementInterval.HasValue)
      {
        //run timer to increment progress bar
        progressTimer.Interval = TimeSpan.FromMilliseconds(autoIncrementInterval.Value);
        progressTimer.Start();
      }

      //store the UI culture
      uiCulture = CultureInfo.CurrentUICulture;

      //store reference to callback handler and launch worker thread
      workerCallback = workHandler;
      worker.RunWorkerAsync(argument);

      //display modal dialog (blocks caller)
      return ShowDialog() ?? false;
    }

    #endregion


    #region event handlers

    /// <summary>
    /// Worker method that gets called from a worker thread.
    /// Synchronously calls event listeners that may handle
    /// the work load.
    /// </summary>
    private void worker_DoWork(object sender, DoWorkEventArgs e)
    {
      try
      {
        //make sure the UI culture is properly set on the worker thread
        Thread.CurrentThread.CurrentUICulture = uiCulture;

        //invoke the callback method with the designated argument
        workerCallback(sender, e);
      }
      catch (Exception)
      {
        //disable cancelling and rethrow the exception
        
        throw;
      }
    }


    /// <summary>
    /// Cancels the background worker's progress.
    /// </summary>
    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      worker.CancelAsync();
      cancelled = true;
    }


    /// <summary>
    /// Visually indicates the progress of the background operation by
    /// updating the dialog's progress bar.
    /// </summary>
    private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      if (!Dispatcher.CheckAccess())
      {
        //run on UI thread
        ProgressChangedEventHandler handler = worker_ProgressChanged;
        Dispatcher.Invoke(DispatcherPriority.SystemIdle, handler, new object[] {sender, e}, null);
        return;
      }

      if (e.ProgressPercentage != int.MinValue)
      {
        progressBar.Value = e.ProgressPercentage;
        txtDialogMessage.Text=e.UserState.ToString();
      }

    }


    /// <summary>
    /// Updates the user interface once an operation has been completed and
    /// sets the dialog's <see cref="Window.DialogResult"/> depending on the value
    /// of the <see cref="AsyncCompletedEventArgs.Cancelled"/> property.
    /// </summary>
    private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if (!Dispatcher.CheckAccess())
      {
        //run on UI thread
        RunWorkerCompletedEventHandler handler = worker_RunWorkerCompleted;
        Dispatcher.Invoke(DispatcherPriority.SystemIdle, handler, new object[] {sender, e}, null);
        return;
      }

      if (e.Error != null)
      {
        error = e.Error;
      }
      else if (!e.Cancelled)
      {
        //assign result if there was neither exception nor cancel
        result = e.Result;
      }

      //update UI in case closing the dialog takes a moment
      progressTimer.Stop();
      progressBar.Value = progressBar.Maximum;

      //set the dialog result, which closes the dialog
      DialogResult = error == null && !e.Cancelled;
    }


    /// <summary>
    /// Periodically increments the value of the progress bar.
    /// </summary>
    private void OnProgressTimer_Tick(object sender, EventArgs e)
    {
      int threshold = 100 + progressBarIncrement;
      progressBar.Value = ((progressBar.Value + progressBarIncrement)%threshold);
    }

    #endregion


    #region update progress bar / status label

    /// <summary>
    /// Directly updates the value of the underlying
    /// progress bar. This method can be invoked from a worker thread.
    /// </summary>
    /// <param name="progress"></param>
    /// <exception cref="ArgumentOutOfRangeException">If the
    /// value is not between 0 and 100.</exception>
    public void UpdateProgress(int progress)
    {
      if (!Dispatcher.CheckAccess())
      {
        //switch to UI thread
        Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                     (SendOrPostCallback)
                                     delegate { UpdateProgress(progress); }, null);
        return;  
      }


      //validate range
      if (progress < progressBar.Minimum || progress > progressBar.Maximum)
      {
        string msg = "Only values between {0} and {1} can be assigned to the progress bar.";
        msg = String.Format(msg, progressBar.Minimum, progressBar.Maximum);
        throw new ArgumentOutOfRangeException("progress", progress, msg);
      }

      //set the progress bar's value
      progressBar.SetValue(ProgressBar.ValueProperty, progress);
    }


    /// <summary>
    /// Sets the content of the status label to a given value. This method
    /// can be invoked from a worker thread.
    /// </summary>
    /// <param name="status">The status to be displayed.</param>
    public void UpdateStatus(object status)
    {
    }

    #endregion


    #region invoke methods on UI thread

    /// <summary>
    /// Asynchronously invokes a given method on the thread
    /// of the dialog's dispatcher.
    /// </summary>
    /// <param name="method">The method to be invoked.</param>
    /// <param name="priority">The priority of the operation.</param>
    /// <returns>The result of the
    /// <see cref="Dispatcher.BeginInvoke(DispatcherPriority,Delegate)"/>
    /// method.</returns>
    public DispatcherOperation BeginInvoke(Delegate method, DispatcherPriority priority)
    {
      return Dispatcher.BeginInvoke(priority, method);
    }


    /// <summary>
    /// Synchronously invokes a given method on the thread
    /// of the dialog's dispatcher.
    /// </summary>
    /// <param name="method">The method to be invoked.</param>
    /// <param name="priority">The priority of the operation.</param>
    /// <returns>The result of the
    /// <see cref="Dispatcher.Invoke(DispatcherPriority,Delegate)"/>
    /// method.</returns>
    public object Invoke(Delegate method, DispatcherPriority priority)
    {
      return Dispatcher.Invoke(priority, method);
    }

    #endregion
  }
}