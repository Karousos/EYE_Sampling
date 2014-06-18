using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace Evolve.Wpf.Samples
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class Window1 : Window
  {
    public Window1()
    {
      InitializeComponent();
    }


    /// <summary>
    /// Triggers an operation that just goes on until the
    /// user cancels. The progress indication is automatically
    /// incremented by the progress bar every 150ms.
    /// </summary>
    private void btnShowWithCancelButton_Click(object sender, RoutedEventArgs e)
    {
      //the basic configuration expects the progress bar to be updated
      //by the worker thread
      ProgressDialog dlg = new ProgressDialog();
      dlg.Owner = this;
      dlg.DialogText = txtDialogTitle.Text;

      //allow cancelling, and activate automatic
      //progress indication every 150 millisecons
      dlg.IsCancellingEnabled = true;
      dlg.AutoIncrementInterval = 150;

      //we cannot access the user interface on the worker thread, but we
      //can submit an arbitrary object to the RunWorkerThread method
      int startValue = int.Parse(txtStartValue.Text);

      //start processing and submit the start value
      dlg.RunWorkerThread(startValue, CountUntilCancelled);
    }


    /// <summary>
    /// This method will be invoked on a background thread by the
    /// ProgressDialog control. It increments the reference value
    /// until the user presses the cancel button.
    /// </summary>
    /// <param name="sender">The background worker component</param>
    /// <param name="e">Provides the start value as the argument.</param>
    private void CountUntilCancelled(object sender, DoWorkEventArgs e)
    {
      //the sender property is a reference to the dialog's BackgroundWorker
      //component
      BackgroundWorker worker = (BackgroundWorker) sender;

      //the start value was submitted as an argument
      int startValue = (int) e.Argument;
      while (!worker.CancellationPending)
      {
        startValue++;

        //report a progress of int.MinValue: This does not affect the progress
        //bar, but the text will be updated
        worker.ReportProgress(int.MinValue, "We're at " + startValue);

        Thread.Sleep(100);
      }
    }


    private void btnShowWithoutCancelButton_Click(object sender, RoutedEventArgs e)
    {
      //the basic configuration expects the progress bar to be updated
      //by the worker thread
      ProgressDialog dlg = new ProgressDialog();
      dlg.Owner = this;
      dlg.DialogText = txtDialogTitle.Text;

      //we cannot access the user interface on the worker thread, but we
      //can submit an arbitrary object to the RunWorkerThread method
      int startValue = int.Parse(txtStartValue.Text);

      //start processing and submit the start value
      dlg.RunWorkerThread(startValue, CountTo100);
    }


    /// <summary>
    /// This method will be invoked on a background thread by the
    /// ProgressDialog control. It increments the reference value 100 times,
    /// then aborts by itself.
    /// </summary>
    /// <param name="sender">The background worker component</param>
    /// <param name="e">Provides the start value as the argument.</param>
    private void CountTo100(object sender, DoWorkEventArgs e)
    {
      //the sender property is a reference to the dialog's BackgroundWorker
      //component
      BackgroundWorker worker = (BackgroundWorker) sender;
      
      //the start value was submitted as an argument
      int startValue = (int) e.Argument;

      for (int i = 0; i < 100; i++)
      {
        //increment the value
        startValue++;
        string msg = "Value is {0} - we've already processed {1} %...";
        msg = String.Format(msg, startValue, i);
        worker.ReportProgress(i, msg);

        //make a short break
        Thread.Sleep(50);

        //if the user cancelled, break
        if (worker.CancellationPending) break;
      }
    }


    #region generate worker thread exception

    private void btnShowWithoutProgressBar_Click(object sender, RoutedEventArgs e)
    {
      //the basic configuration expects the progress bar to be updated
      //by the worker thread
      ProgressDialog dlg = new ProgressDialog();
      dlg.Owner = this;
      dlg.DialogText = txtDialogTitle.Text;
      dlg.AutoIncrementInterval = 100;

      //we cannot access the user interface on the worker thread, but we
      //can submit an arbitrary object to the RunWorkerThread method
      int startValue = int.Parse(txtStartValue.Text);

      //start processing and submit the start value
      dlg.RunWorkerThread(startValue, DivideByZero);

      if (dlg.Error != null)
      {
        String msg = "An error occurred on the worker thread:\n\n {0}";
        msg = String.Format(msg, dlg.Error.ToString());
        MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }


    private void DivideByZero(object sender, DoWorkEventArgs e)
    {
      //the sender property is a reference to the dialog's BackgroundWorker
      //component
      BackgroundWorker worker = (BackgroundWorker)sender;

      //the start value was submitted as an argument
      int refValue = (int)e.Argument;

      for (int i = 5; i >=0; i--)
      {
        //perform division
        int result = refValue/i;
        
        string msg = "{0} divided by {1} is {2}.";
        msg = String.Format(msg, refValue, i, result);
        worker.ReportProgress(int.MinValue, msg);

        //make a break
        Thread.Sleep(1500);

        //if the user cancelled, break
        if (worker.CancellationPending) break;
      }

      MessageBox.Show("We'll never get here as we're dividing by zero in the loop above");
    }

    #endregion

  }
}