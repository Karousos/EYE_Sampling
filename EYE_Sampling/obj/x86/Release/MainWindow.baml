   M S B A M L   `   `   `  ����   EYE_Sampling    EYE_Sampling.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -     �Name$  Window_Main�� � Title$ ΕΥΕ Sampling��$	��708��$
��1087�� � WindowStartupLocation$ CenterScreen=�$	��535��.��� .+���  �� TabStripPlacement$
 Left=�$��0,0,0,0q���Z�  � 	Resources  �� 
TargetType �� Property �� Value �� 
TargetType	 ��Name
 ��Name �� Orientation �� Triggers P� Property P� Value �� 
TargetName��%  '��       �� " ��.���  � �� �� " ��.R�� .+��� $	 Border��$��0,0,0,0q�
������ 
���   �._��� $
 ContentSite��$��Center=�$��Center=�$��Header��$��	12,2,12,2q���. �� $ 
Horizontal=�.+��� $�� ��$��2,0,0,0q�$��Center=� P�  �@��
IsSelected . . ���  �@K�ZIndex$	 100����  � �� �� .R��� $
��Bold"����� P�  �@J�IsMouseOver . . ���  �@K�ZIndex$	 100���� $ Border�� � ��$ 2,2,2,2q��� $ Border�� � ��
 ����� J� BitmapEffect F�  F� Color$
 Navy�� F� Opacity$	 0.3Q�.@��� $%��Δεδομένα Εισόδου��$
��Left=�$��0q�$	��Top=�$	��140��$��30��.��� 
�������.+�&   System.Windows.Controls.DataGrid  -     Name$ DG_Items��$��10,84,10,10q�
�������   AutoGenerateColumns .    
IsReadOnly .  J� IsManipulationEnabled . ���� *Data+�� $,��&Νέα δεδομένα εισόδου��$	��Top=�$��Italic$�$	��360��$��10,10,559,0q��� $k��eΕπιλέξτε αρχείο εισόδου xlsx για την εισαγωγή νέων έργων.��$	��Top=�$��
0,19,441,0q�$��Right=�$	��330���� -    ��Name$ BT_Mappings��$��32��$	��Top=�$��0,0,0,0q�
������ $��
0,16,366,0q�$��Right=�$��38��.���� $ 
Horizontal=�$��0q�.+��� $��images/mappings.jpg���� -   $ BT_FileImport��$-��'Φόρτωμα Προγραμμάτων��$��32��$	��Top=�$��0,0,0,0q�
������ $��
0,16,209,0q�$��Right=�$	��152���� -   $ BT_Mappings_Check��$��32��$	��Top=�$��0,0,0,0q�
������ $��
0,16,128,0q�$��Right=�$��38��.���� $ 
Horizontal=�$��0q�.+��� $��images/mappings.jpg���� -   $ BT_FileImport_Checks��$#��Φόρτωμα Ελέγχων��$��32��$	��Top=�$��0,0,0,0q�
������ $��	0,16,10,0q�$��Right=�$	��113��{�.  �� $,��&Συνολικός Πληθυσμός:��$	��Top=�$��	10,53,0,0q�$��Italic$�$
��Left=�$	��137���� -    ��Name$ LB_total_items��$	��Top=�$��
140,53,0,0q�$��Italic$�$
��Left=�$��46��$
��Bold"�$��0���� $��MIS Project:��$	��Top=�$��
180,53,0,0q�$��Italic$�$
��Left=�$��70���� -    ��Name$ TB_MIS��$��23��$��
250,53,0,0q� �� TextWrapping$
 Wrap=�$	��Top=�$
��Bold"�$
��Left=�$��89���� $��Τίτλος Project:��$	��Top=�$��
343,53,0,0q�$��Italic$�$
��Left=�$��86���� -	   $ TB_Title��$��23��$��428,53,10,0q�$
 Wrap=�$	��Top=�$
��Bold"��� $��Ρυθμίσεις��$
��Left=�$��0q�$	��Top=�$	��140��$��30��.��� 
�������$��0,0,0,6q�.+��� $9��3Ζητούμενο πλήθος δειγμάτων:��$
��Left=�$��	43,11,0,0q�$	��Top=��� -
   $ 
TB_Percent��$
��Left=�$��23��$��
219,14,0,0q�$
 Wrap=�$��20��$	��Top=�$��41��$��1,1,1,1q�
������ 
����  �$
��Bold"��� $
��% ή��$
��Left=�$��
259,11,0,0q�$	��Top=��� -   $ TB_NumOfSamples��$
��Left=�$��23��$��
287,14,0,0q�$
 Wrap=�$��40��$	��Top=�$��37��$��1,1,1,1q�
������ 
����  �$
��Bold"��� $��δείγματα.��$
��Left=�$��
329,11,0,0q�$	��Top=��� $-��'Ρυθμίσεις Συμμετοχής��$
��Left=�$��	43,46,0,0q�$	��Top=����	#FFB4A8A8�� $,��&Συνολικός πληθυσμός:��$��Right=�$��	0,41,88,0q�$	��Top=�$	��134��$��Right=��� -   $ LB_Participation_Total��$��0��$��Right=�$��	0,41,51,0q�$	��Top=�$��37��$
��Left=�$
��Bold"��� 
���   �$��1q�$��
47,77,51,0q�$	��280��$	��Top=�._�� $��9q�
������  � ColumnDefinitions �� $	��91* �.+��� -    ��Name$ CB_Praxi��$q��kΝα συμμετέχουν τα projects που υλοποιούνται στις εξής πράξεις:��$
��Left=�$��	11,10,0,0q�$	��Top=��� -   $ BT_Praxi��$	��...��$��26��$	��Top=�$��0,0,0,0q�
������ $��	391,5,0,0q�$
��Left=�$��24��{�.  �� -   $ 
LB_Praxeis��$�� ��$	��Top=�$��
419,5,10,0q��� -   $ 	CB_Region��$s��mΝα συμμετέχουν τα projects που υλοποιούνται στις εξής περιοχές:��$
��Left=�$��	11,44,0,0q�$	��Top=��� -   $ 	BT_Region��$	��...��$��26��$	��Top=�$��0,0,0,0q�
������ $��
391,39,0,0q�$
��Left=�$��24��{�.  �� -   $ CB_Date��$O��IΝα είναι σε εξέλιξη το χρονικό διάστημα:��$
��Left=�$��	11,77,0,0q�$	��Top=�(  "System.Windows.Controls.DatePicker  -     Name$ DP_Start��$��
254,73,0,0q�$��27��$	��Top=�$
��Left=�$	��106��{�.  �� $��έως��$
��Left=�$	��Top=�$��31��$��
365,72,0,0q�  -   $ DP_Stop��$��
401,71,0,0q�$��28��$	��Top=�$
��Left=�$	��106��{�.  �� -   $ CB_Money��$F��@Ο προυπολογισμός τους να είναι από:��$
��Left=�$��
11,110,0,0q�$	��Top=��� -   $ TB_Money_From��$
��Left=�$��23��$��240,108,0,0q�$
 Wrap=�$��0��$	��Top=�$��65��$��1,1,1,1q�
������ 
����  �$
��Bold"�{�.  �� $��έως��$
��Left=�$	��Top=�$��33��$��308,105,0,0q��� -   $ TB_Money_To��$
��Left=�$��23��$��346,108,0,0q�$
 Wrap=�$��50000��$	��Top=�$��87��$��1,1,1,1q�
������ 
����  �$
��Bold"�{�.  �� $��ευρώ��$
��Left=�$	��Top=�$��41��$��433,108,0,0q��� -   $ CB_Theoritical��$)��#Να αφορούν μόνο σε :��$
��Left=�$��
11,141,0,0q�$	��Top=��� -    ��Name$ Combo_Theoritical��$
��Left=�$��154,138,0,0q�$	��Top=�$	��158��$��0,0,0,0q�
������ 
����  � �� SelectedIndex$ 0��{�.  .@��� $+��%Θεωρητική Κατάρτιση���� $#��Πρακτική Άσκηση���� -   $ CB_Students��$����Ο αριθμός των ασκούμενων στην επιχείρηση  πρακτικής που θα ελεγχθεί να είναι  μεγαλύτερος του:��$
��Left=�$��
11,172,0,0q�$	��Top=��� -   $ TB_Students��$
��Left=�$��25��$��587,169,0,0q�$
 Wrap=�$��20��$	��Top=�$��35��$��1,1,1,1q�
������ 
����  �$
��Bold"�{�.  �� -   $ CB_Theoritical_Copy��$+��%Τύπος φορέα/παρόχου:��$
��Left=�$��
11,200,0,0q�$	��Top=�{�.  �� -   $ Combo_Theoritical_Copy��$
��Left=�$��161,197,0,0q�$	��Top=�$	��158��$��0,0,0,0q�
������ 
����  �$ 0��{�.  .@��� $��
Δήμος���� $��Επιχείρηση���� -   $ CB_Anadoxoi��$r��lΝα μή συμμετέχουν ανάδοχοι που έχουν ελεγχθεί τουλάχιστον:��$
��Left=�$��
11,228,0,0q�$	��Top=�$��16��$	��368���� -   $ TB_Anadoxoi��$
��Left=�$��23��$��388,227,0,0q�$
 Wrap=�$��2��$	��Top=�$��30��$��1,1,1,1q�
������ 
����  �$
��Bold"��� -    $ LB_v1a_Copy2��$��φορές.��$
��Left=�$��419,224,0,0q�$	��Top=�{�.  $��26��$��46���� $0��*Εναπομείνας Πληθυσμός:��$��Right=�$��
0,357,88,0q�$	��Top=�$	��207��$��Right=��� -!   $ LB_Participation_Rest��$��0��$��Right=�$��
0,357,51,0q�$	��Top=�$��37��$
��Left=�$
��Bold"��� $B��<Ρυθμίσεις Υποχρεωτικής Επιλογής��$
��Left=�$��
47,401,0,0q�$	��Top=����	#FFB4A8A8�� 
���   �$��1q�$��90��$	��Top=�$��47,432,51,0q�._�� $��9q�
������  �� $	��59* ��� $
��454* �.+��� -"   $ CB_Yp_Anadoxoi��$j��dΝα επιλεγούν ανάδοχοι που έχουν ελεγχθεί λιγότερο από ��$
��Left=�$��	10,10,0,0q�$	��Top=�$��2��$��16��$	��334���� -#   $ TB_Yp_Anadoxoi��$
��Left=�$��23��$��	260,8,0,0q�$
 Wrap=�$��1��$	��Top=�$��30��$��1,1,1,1q�
������ 
����  �$
��Bold"�$��1���� -$   $ LB_v1a_Copy��$��φορές.��$
��Left=�$��	291,5,0,0q�$	��Top=�{�.  $��1��$��26��$��46���� -%   $ CB_Yp_Poines��$q��kΝα επιλεγούν ανάδοχοι που έχουν επιβληθεί ποινές πάνω από ��$
��Left=�$��	10,47,0,0q�$	��Top=�$��2��$��16��$	��362���� -&   $ TB_Yp_Poines��$
��Left=�$��23��$��
286,44,0,0q�$
 Wrap=�$��50000��$	��Top=�$��76��$��1,1,1,1q�
������ 
����  �$
��Bold"�$��1���� -'   $ LB_v1a_Copy1��$��	ευρώ.��$
��Left=�$��
367,42,0,0q�$	��Top=�{�.  $��1��$��26��$��46���� $-��'Τύπος δειγματοληψίας��$
��Left=�$��
47,568,0,0q�$	��Top=����	#FFB4A8A8�� 
���   �$��1q�$��55��$	��Top=�$��47,594,51,0q�._�� $��9q�
������  �� $	��59* ��� $
��454* �.+��� -(   $ LB_v2a1��$�� ��$
��Left=�$��	10,38,0,0q�$	��Top=�{�.  $��2��$��26��$	��212��	� -)     	�Name$  	RB_Random��$��Τυχαία��$
��Left=�$��	10,10,0,0q�$	��Top=�$��74��! d� 	IsChecked$
! TrueY�" 	� 	GroupName$" GRB_Type��	� -*   $  RB_MUS��$8��2Με βαρύτητα προυπολογισμού��$
��Left=�$��	46,10,0,0q�$	��Top=�$	��317��$��1��$" GRB_Type���� $0��*Εναπομείνας Πληθυσμός:��$��Right=�$��
0,527,88,0q�$	��Top=�$	��153��$��Right=��� -+   $ LB_Selection_Rest��$��0��$��Right=�$��
0,527,51,0q�$	��Top=�$��37��$
��Left=�$
��Bold"��� $$��Επιλεγμένα Έργα:��$��Right=�$��0,527,281,0q�$	��Top=�$	��153��$��Right=��� -,   $ LB_Selection��$��0��$��Right=�$��0,527,244,0q�$	��Top=�$��37��$
��Left=�$
��Bold"��� $ ��Δειγματοληψία��$
��Left=�$��0q�$	��140��$��30��$	��Top=�.��� 
�������.+��� $N��HΣύνολο προγραμμάτων για δειγματοληψία:��$
��Left=�$��9,34,0,0q�$	��Top=��� --   $ LB_Sampling_ForSampling��$�� ��$
��Left=�$��
254,34,0,0q�$	��Top=�$��67�����	#FF738AE8$
��Bold"��� $L��FΖητούμενα δείγματα από δειγματοληψία:��$
��Left=�$��
385,34,0,0q�$	��Top=��� -.   $ LB_NumOfRestSamples��$�� ��$
��Left=�$��
619,34,0,0q�$	��Top=�$��46�����	#FF738AE8$
��Bold"��� -/   $ BT_LoadProjects��$��	0,28,11,0q�$��Right=�$	��177��$��0,0,0,0q�
������ 
����  �$��32��$	��Top=�.���� $ 
Horizontal=�$��0q�.+��� $��3q�$-��'Φόρτωση προγραμμάτων��$��Center=��� $��images/refresh.jpg��  -0   $ DG_Input_Projects��$��	9,65,11,0q�
������� . $	��180��$	��Top=��� -1   $ BT_Start��$/��)Έναρξη Δειγματοληψίας��$��32��$	��Top=�$��0,0,0,0q�
������ $��
10,250,0,0q�$
��Left=�$	��148�����	#FFD8D8D8  -2   $ DG_Output_Projects��$��10,287,10,45q�
������� . �� -3   $ BT_Original��$��	0,0,534,8q�$��Right=�$	��155��$��0,0,0,0q�
������ 
����  �$��32��$��Bottom=�.���� $ 
Horizontal=�$��0q�.+��� $��3q�$'��!Αρχικός πληθυσμός���� $��images/excelicon.png���� -4   $ BT_Sampling��$��	0,0,315,8q�$��Right=�$	��214��$��0,0,0,0q�
������ 
����  �$��32��$��Bottom=�.���� $ 
Horizontal=�$��0q�.+��� $��3q�$<��6Πληθυσμός προς δειγματοληψία���� $��images/excelicon.png���� -5   $ 	BT_Output��$��	0,0,168,8q�$��Right=�$	��142��$��0,0,0,0q�
������ 
����  �$��32��$��Bottom=�.���� $ 
Horizontal=�$��0q�.+��� $��3q�$#��Επιλεγμένα Έργα���� $��images/excelicon.png���� -6   $ 	BT_Report��$��0,0,11,8q�$��Right=�$	��152��$��0,0,0,0q�
������ 
����  �$��32��$��Bottom=�.���� $ 
Horizontal=�$��0q�.+��� $��3q�$��Αναφορά���� $��images/doc.png���� -7   $ LB_Green��$F��@Πράσινα: Επιλεγμένα λόγω ρυθμίσεων��$
��Left=�$��279,256,0,0q�$	��Top=�$	��224��
��� � �$y�Hidden=��� -8   $ LB_Blue��$F��@Μπλε: Επιλεγμένα από δειγματοληψία��$
��Left=�$��523,256,0,0q�$	��Top=�$	��224��
����  �$y�Hidden=��� -9   $ CB_Test��$��(Δοκιμαστική)��$
��Left=�$��163,258,0,0q�$	��Top=�# J� RenderTransformOrigin$# 0.853,1.125&�$	��178���� $,��&Συνολικός Πληθυσμός:��$
��Left=�$��9,10,0,0q�$	��Top=��� -:   $ LB_Sampling_All��$�� ��$
��Left=�$��
141,10,0,0q�$	��Top=�$��52�����	#FF738AE8$
��Bold"��� $*��$Ζητούμενα δείγματα:��$
��Left=�$��
203,10,0,0q�$	��Top=��� -;   $ LB_Sampling_Asked��$�� ��$
��Left=�$��
321,10,0,0q�$	��Top=�$��42�����	#FF738AE8$
��Bold"��� $,��&Επιλεγμένα δείγματα:��$
��Left=�$��
384,10,0,0q�$	��Top=��� -<   $ LB_Sampling_Selected��$�� ��$
��Left=�$��
509,10,0,0q�$	��Top=�$��46�����	#FF738AE8$
��Bold"��� $��Αναφορές��$
��Left=�$��0,-3,0,0q�$	��Top=�$	��140��$��30��.��� 
�������.+��� -=   $ ��Name$$ 
LV_History��$��	0,34,0,51q�
���   �% %� IsVirtualizing% . & %� VirtualizationMode$& 	Recycling=�!' �� IsDeferredScrollingEnabled' . ( �� View( �� .P��� ) �� Width$	) 160��$��Project��* �� DisplayMemberBinding* �� *ProjectTitle+�� $	) 160��$��Ημερομηνία��* �� *ProjectDateTime+�� $�� ��$	) 200��+ �� CellTemplate+ �� .R��� ->   $��5,0,0,0q�$��Right=�$	��162��$��0,0,0,0q�
������ 
����  �$��32��$��Bottom=����� .���� $ 
Horizontal=�$��0q�.+��� $��3q�$-��'Αρχείο Συμμετεχόντων���� $��images/excelicon.png���� $�� ��$	) 200��+ �� .R��� -?   $��5,0,0,0q�$��Right=�$	��162��$��0,0,0,0q�
������ 
����  �$��32��$��Bottom=����� .���� $ 
Horizontal=�$��0q�.+��� $��3q�$)��#Αρχείο Επιλεγμένων���� $��images/excelicon.png���� $�� ��$	) 200��+ �� .R��� -@   $��5,0,0,0q�$��Right=�$	��162��$��0,0,0,0q�
������ 
����  �$��32��$��Bottom=����� .���� $ 
Horizontal=�$��0q�.+��� $��3q�$*��$Αναφορά δειγμα/ψίας���� $��images/doc.png���� $3��-Ιστορικό Δειγματοληψιών��$��0,8,0,0q�$	��Top=�$
��Left=�$	��150���� $��	0,0,10,10q�$��Right=�$��0,0,0,0q�
������ 
����  �$��32��$��Bottom=�{�.  .���� $ 
Horizontal=�$��0q�.+��� $��3q�$K��EΑναλυτικό αρχείο κινήσεων Συστήματος���� $��images/txt_file.png���� $��
Περί..��$
��Left=�$��0q�$	��Top=�$	��140��$��30��.��� 
�������.+��� $��1,1,1,1q�
�������$��209,173,0,0q�$
��Left=�$	��537��$	��211��$	��Top=�._�� $��9q�
������ .+��� $��EYE Sampling V1.02��$
��Left=�$��
268,10,0,0q�$	��Top=����	#FF152DC3$
��Bold"��� $��	16-3-2014��$
��Left=�$��
298,36,0,0q�$	��Top=����	#FF152DC3$��0,0,0,0q��� $��Developed by��$��268,62,42,0q�$��0,0,0,0q�$��17��$	��Top=��� $��Nikos Karousos��$��
268,77,0,0q����	#FF152DC3$��0,0,0,0q�$��16��$	��Top=�$
��Left=�$��95���� $��karousos@gmail.com��$��
288,93,0,0q����	#FFA7AAB9$��10(�$��0,0,0,0q�$��14��$	��Top=�$
��Left=�$	��104���� $��Consulted by��$��268,116,0,0q�$��0,0,0,0q�$��17��$	��Top=�$
��Left=�$��82���� $��Nikos Karacapilidis��$��268,130,0,0q����	#FF152DC3$��0,0,0,0q�$��24��$	��Top=�$
��Left=�$	��115���� $��nikos@mech.upatras.gr��$��280,147,0,0q����	#FFA7AAB9$��10(�$��0,0,0,0q�$��21��$	��Top=�$
��Left=�$	��112��]� ��.  $��1,5,2,1q�, J� OpacityMask
, �   �.+��� $	��171��$	��Top=�$	��215��$��images/target.jpg���� $��Καλωσήρθες��$��	10,0,0,36q����	#FFA29C9C$
��Left=�$	��126��$��37��$��Bottom=��� -A   $ LB_User��$�� ��$
��Left=�$��	10,0,0,10q�$	��126�����	#FF152DC3$��Right=�$��36��$��Bottom=�