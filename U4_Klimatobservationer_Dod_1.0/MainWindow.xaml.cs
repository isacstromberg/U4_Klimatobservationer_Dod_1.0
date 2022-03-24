using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using U4_Klimatobservationer_Dod_1._0.Repostories;
using U4_Klimatobservationer_Dod_1._0.Models;

namespace U4_Klimatobservationer_Dod_1._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Db_Repository db = new Db_Repository();
        Polymorph_Tools po = new Polymorph_Tools();
    


        public MainWindow()
        {
            InitializeComponent();
            var db = new Db_Repository();
          
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //lägg till observatör
            string Firstname_parameter = Firstname_input.Text;
            string Lastname_parameter = Lastname_input.Text;
            if (db.AddObserver(Firstname_parameter, Lastname_parameter) == true)
            {
                MessageBox.Show("Det gick bra den nya observatören har nu lagts till!!");
            }
            else
            {
                MessageBox.Show("Det gick inte bra, du måste ange rätt format. förnamn och efternamn måste bestå av text");
            }
           
        }

        private void Firstname_input_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ListBox_Observer.ItemsSource = null;
            try
            {
                if (db.CheckIfObserverDBEmpty() == true)
                {
                    MessageBox.Show("Databasen är tömd på observatörer!!! Fyll på med nya observatörer och hämta observatörerna igen."); return;
                }
                ListBox_Observer.ItemsSource = db.GetObservers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
      
          
        public void ListBox_Observer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var ob = new Observer();
            Observer observer = ListBox_Observer.SelectedItem as Observer;
            db.observers.Add(observer);
            po.SetSelectedObserverValues(observer.firstname, observer.lastname, observer.id);
            lblObserver_Name.Content = null;
            lblObserver_Name.Content = po.SelectedFullName;
            MessageBox.Show($"Du har nu valt {po.SelectedFirstName} {po.SelectedLastName} med id {po.SelectedID}");
        }

        public void LstMeasurment_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var ob = new Observation();
            Measurement measurement = LstMeasurment.SelectedItem as Measurement;
            po.SelectedMeasurementID = measurement.id;
            TxtEditValue.Text = $"Ange nytt mätvärde för mätpunkt {po.SelectedMeasurementID}";
            MessageBox.Show($"Du har nu valt att redigera mätpunkten med ID: {po.SelectedMeasurementID}.");
        }
        public void lstObservations_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var ob = new Observation();
            var db = new Db_Repository();
            Observation observation = lstObservations.SelectedItem as Observation;
            db.observations.Add(observation);
            po.SelectedObservationID = observation.id;
            po.SelectedObservationDate = observation.date;
            po.SelectedObserver_ID = observation.observer_id;
            po.SelectedGeolocation_ID = observation.geolocation_id;
            db.GetMeasurementObservation_id();
            foreach (var item in db.GetMeasurementObservation_id())
            {
                if (item.observation_id == po.SelectedObservationID)
                {
                    po.SelectedObservationID = item.observation_id;
                }
            }
            string SelectedObservationIDString = po.SelectedObservationID.ToString();
            LstMeasurment.ItemsSource = null;
            LstMeasurment.ItemsSource = db.GetObservationReportedMeasurements(po.SelectedObservationID);
            TxtEditValue.Text = $"Du har nu valt att redigera mätpunkter från observation med ID: {SelectedObservationIDString}. Välj mätpunkt via dubbelklick för redigering";
            MessageBox.Show($"Du har nu valt observation {po.SelectedObservationID} {po.SelectedObservationDate} med observatören {po.SelectedObserver_ID}");
        }

        private void ListBox_Observer_Copy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

            //  Här är koden för att genomföra 
            // Ta bort en befintlig observatör. Om denne har genomfört observationer 
            // ska man få ett relevant felmeddelande

        

            if (db.BlockLetters(Txt_Observer_ID.Text) == false)
            {
                MessageBox.Show("Du får enbart ange siffror");
                return;
            }  
            int ID_parameter = Convert.ToInt32(Txt_Observer_ID.Text);
            string ID_parameter_string = ID_parameter.ToString();
          
           if (db.CheckIfObserverHasDoneObservation(ID_parameter) == true)
            {
                MessageBox.Show("Stopp! Denna observatör har genomfört observationer och kan därför inte tas bort."); return;
            }
            if (db.CheckIfInput_ID_MatchesObserverID(ID_parameter) == false)
            {
                MessageBox.Show("Stopp! Det finns ingen observatör i databasen som har det ID du angett"); return;
            }
            


            try
                {
                    db.RemoveObserver(ID_parameter);
                    MessageBox.Show($"Det gick bra, observatören med ID {ID_parameter_string} är nu bortagen!");
                }
                catch (Exception ex)
                {
                   MessageBox.Show(ex.Message);
             
                }
           
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            double Value_Parameter = Convert.ToDouble(txtValue.Text);
            double Value2_Parameter = Convert.ToDouble(txtValue_2.Text);
            double? GetGeolocation = db.SelectGeolocation();
            if (radSnowDepthAndTemp.IsChecked == false)
            {
                MessageBox.Show("Du måste välja en kategori!!");
            }
            db.RegisterObservation(po.SelectedID, GetGeolocation);
            db.CatchCreatedObservationID();
            if (radSnowDepthAndTemp.IsChecked == true)
            {
                db.RegisterMeasurement(Value_Parameter, db.CatchCreatedObservationID(), 8);
                db.RegisterMeasurement(Value2_Parameter, db.CatchCreatedObservationID(), 5);
            }
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            lstObservations.ItemsSource = null;
            lstObservations.ItemsSource = db.GetObserverReportedObservation(po.SelectedID);
        }

        private void lstObservations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {

            if (db.BlockLetters(TxtEditValue.Text) == false)
            {
                MessageBox.Show("Du får enbart ange siffror");
                return;
            }

            double EditedValue = Convert.ToDouble(TxtEditValue.Text);
            db.UpdateObservationMeasurement(EditedValue,po.SelectedMeasurementID);
        }
    }
}
