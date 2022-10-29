using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection.Emit;

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
            count11.Visibility = Visibility.Visible;
            count22.Visibility = Visibility.Visible;
            count33.Visibility = Visibility.Visible;
            con.Open();

            int discharge,timeframe;
            int temp, temp1;
            int min, min1;
            int cycle = 0;
            string[] datehour=new string[] { };
            //List<string> hours=new List<string>();
            DataTable dt;
            dt = new DataTable();
            dt.Columns.Add("Day");
            dt.Columns.Add("Time Frame");
            dt.Columns.Add("Discharge Time");
            dt.Columns.Add("Discharge");

            var time = DateTime.Now;
            DateTime dt2 = time.AddDays(-6);
            for (int i = 1; i <= 5; i++)
            {
                dt2 = dt2.AddDays(1);
                string formatted = dt2.ToString("dd-MM-yyyy");

                discharge = 0;
                timeframe = 0;
                SQLiteCommand cmd = new SQLiteCommand("select distinct strftime('%H',timenow) from Batteryinfo where  strftime('%d-%m-%Y',timenow)='" + formatted + "' and batterystatus=1;", con);
                SQLiteDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        datehour =datehour.Append(rd[0].ToString()).ToArray();

                    }

                    foreach (string hour in datehour)
                    {

                        discharge = 0;
                        timeframe = 0;
                        SQLiteCommand cmd1 = new SQLiteCommand("select batterylevel,strftime('%M',timenow) from Batteryinfo where  strftime('%d-%m-%Y %H',timenow)='" + formatted + " " + hour + "' and batterystatus=1;", con);
                        SQLiteDataReader reader = cmd1.ExecuteReader();
                        if (reader.HasRows)
                        {
                            reader.Read();
                            temp = int.Parse(reader[0].ToString());
                            min = int.Parse(reader[1].ToString());
                            while (reader.Read())
                            {

                                temp1 = int.Parse(reader[0].ToString());
                                min1 = int.Parse(reader[1].ToString());
                                if (temp1 > temp)
                                {
                                    temp = temp1;
                                    min = min1;

                                }
                                else
                                {
                                    discharge += (temp - temp1);
                                    timeframe += (min1 - min);
                                    temp = temp1;
                                    min = min1;
                                }

                            }

                            dt.Rows.Add(formatted, hour + ":00:00", timeframe, discharge);
                            cycle += discharge;

                        }
                        reader.Close();

                    }

                }
            }
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                myDataGrid.Columns.Add(new DataGridTextColumn()
                {
                    Header = dt.Columns[i].ColumnName,
                    Binding = new Binding { Path = new PropertyPath("[" + i.ToString() + "]") }
                });
            }
            var collectionObjects = new System.Collections.ObjectModel.ObservableCollection<object>();
            
            foreach (DataRow row in dt.Rows)
            {
                collectionObjects.Add(row.ItemArray);
            }
            int c = cycle / 100;
            chargecycle.Text = c.ToString();
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
            int badcount = 0, optimalcount = 0, spotcount = 0;
            var time = DateTime.Now;
            DateTime dt2 = time.AddDays(-5);

            string formatted = dt2.ToString("dd-MM-yyyy");

            SQLiteCommand cmd = new SQLiteCommand("select distinct batterystatus,batterylevel,timenow from Batteryinfo where  strftime('%d-%m-%Y',timenow) between '" + formatted + "' and strftime('%d-%m-%Y','now');", con);
            SQLiteDataReader reader = cmd.ExecuteReader();
            reader.Read();
            string status = reader[0].ToString();
            string level=reader[1].ToString();
            DateTime dt = DateTime.Parse(reader[2].ToString());
            DateTime dt1;
            TimeSpan count ;
            int timecount = 0;
            while (reader.Read())
            {
                dt1= DateTime.Parse(reader[2].ToString());
                if (reader[0].ToString() != status && status=="2" && level!="100")
                {
                    spotcount++;

                }
                else if(reader[0].ToString() == status && status == "2" && reader[1].ToString() == "100" && level=="100")
                {
                    
                    count= dt1 - dt;
                    timecount += count.Minutes;
                }
                else
                {

                    if (timecount <= 0)
                    {

                    }
                    else if (timecount < 30)
                    {
                        optimalcount++;
                    }
                    else if (timecount >= 30)
                    {
                        badcount++;
                    }
                    timecount = 0;
                }
                status = reader[0].ToString();
                level= reader[1].ToString();
            }
            reader.Close();

            count11.Text=badcount.ToString();
            count22.Text=optimalcount.ToString();
            count33.Text = spotcount.ToString();
            count1.Visibility = Visibility.Visible;
            count11.Visibility = Visibility.Visible;
            count2.Visibility = Visibility.Visible;
            count22.Visibility = Visibility.Visible;
            count3.Visibility = Visibility.Visible;
            count33.Visibility = Visibility.Visible;

            con.Close();
        }
    }
}
