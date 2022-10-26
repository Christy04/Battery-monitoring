using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Data;
using System.Data.SQLite;


namespace App1
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        SQLiteConnection con = new SQLiteConnection("Data Source=C:\\Users\\cthomas\\Battery.db; Version = 3;New=True;  Compress = True; ");

        public MainWindow()
        {
            this.InitializeComponent();

        }
        void OnClick1(object sender, RoutedEventArgs e)
        {

            count1.Visibility = Visibility.Collapsed;
            count2.Visibility = Visibility.Collapsed;
            count3.Visibility = Visibility.Collapsed;
            con.Open();

            int discharge,timeframe;
            int temp, temp1;
            DataTable dt;
            dt = new DataTable();
            dt.Columns.Add("Day");
            dt.Columns.Add("Time Frame");
            dt.Columns.Add("Discharge Time");
            dt.Columns.Add("Discharge");

            var time = DateTime.Now;
            DateTime dt2 = time.AddDays(-6);
            for (int i = 0; i < 5; i++)
            {
                dt2 = dt2.AddDays(1);
                string formatted = dt2.ToString("dd-MM-yyyy");

                for (int j = 0; j < 25; j++)
                {
                    discharge = 0;
                    timeframe = 0;
                    SQLiteCommand cmd = new SQLiteCommand("select batterylevel from Batteryinfo where  strftime('%d-%m-%Y %H',timenow)='" + formatted + " " + j + "' and batterystatus=1;", con);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        temp = int.Parse(reader[0].ToString());

                        while (reader.Read())
                        {

                            temp1 = int.Parse(reader[0].ToString());
                            if (temp1 > temp)
                            {
                                temp = temp1;

                            }
                            else
                            {
                                discharge += (temp - temp1);
                                timeframe++;
                                temp = temp1;
                            }

                        }
                    }

                    dt.Rows.Add(formatted, j.ToString() + ":00:00",timeframe, discharge);
                }
            }
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                myDataGrid.Columns.Add(new CommunityToolkit.WinUI.UI.Controls.DataGridTextColumn()
                {
                    Header = dt.Columns[i].ColumnName,
                    Binding = new Binding { Path = new PropertyPath("[" + i.ToString() + "]") }
                });
            }
            var collectionObjects = new System.Collections.ObjectModel.ObservableCollection<object>();
            int cycle = 0;
            foreach (DataRow row in dt.Rows)
            {
                cycle += int.Parse(row["Discharge"].ToString());
                collectionObjects.Add(row.ItemArray);
            }
            chargecycle.Text = (cycle / 100).ToString();
            label.Visibility = Visibility.Visible;
            chargecycle.Visibility = Visibility.Visible;
            myDataGrid.ItemsSource = collectionObjects;
            myDataGrid.Visibility = Visibility.Visible;
            con.Close();
        }



        void OnClick2(object sender, RoutedEventArgs e)
        {
            myDataGrid.Visibility = Visibility.Collapsed;
            con.Open();
            int count = 0, badcount = 0, optimalcount = 0, spotcount = 0;
            var time = DateTime.Now;
            DateTime dt2 = time.AddDays(-6);
            for (int i = 0; i < 5; i++)
            {
                count = 0;
                dt2 = dt2.AddDays(1);
                string formatted = dt2.ToString("dd-MM-yyyy");

                SQLiteCommand cmd = new SQLiteCommand("select distinct strftime('%M',timenow),batterystatus from Batteryinfo where  strftime('%d-%m-%Y',timenow)='" + formatted + "' and batterylevel=100 ;", con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        count++;
                    }
                }
                reader.Close();
                if (count == 0)
                {

                }
                else if (count == 1)
                {
                    spotcount++;
                }
                else if(count < 30)
                {
                    optimalcount++;
                }
                else if (count > 30)
                {
                    badcount++;
                }


            }
            count1.Text=badcount.ToString();
            count2.Text=optimalcount.ToString();
            count3.Text = spotcount.ToString();
            count1.Visibility= Visibility.Visible;
            count2.Visibility=Visibility.Visible;
            count3.Visibility = Visibility.Visible;
            con.Close();
        }
    }
}
