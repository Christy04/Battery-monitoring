using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using System.Management;
using System.Data.Entity;

namespace dbcollection
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SQLiteConnection con= new SQLiteConnection("Data Source=C:\\Users\\cthomas\\Battery.db; Version = 3;  Compress = True; ");
            ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_Battery");
            con.Open();
            foreach (ManagementObject mo in mos.Get())
            {
                SQLiteCommand  cmd = new SQLiteCommand("insert into Batteryinfo(batterylevel,batterystatus,timenow) values(" + mo["EstimatedChargeRemaining"] + "," + mo["BatteryStatus"] + ",datetime('now','localtime'))", con);
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }
    }
}
