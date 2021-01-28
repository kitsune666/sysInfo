using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.Runtime.InteropServices;
using System.Management;
using System.Management.Instrumentation;

namespace SysInfo
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //CPU
        PerformanceCounter cpuproc = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        PerformanceCounter cpuproc1 = new PerformanceCounter("Processor", "% Privileged Time", "_Total");
        PerformanceCounter cpuproc2 = new PerformanceCounter("Processor", "% User Time", "_Total");
        PerformanceCounter cpuproc3 = new PerformanceCounter("Processor", "% Interrupt Time", "_Total");
        PerformanceCounter cpuproc4 = new PerformanceCounter("Processor", "% DPC Time", "_Total");


        //MEM
        PerformanceCounter mem = new PerformanceCounter("Memory", "% Committed Bytes In Use", null);
        PerformanceCounter mem1 = new PerformanceCounter("Memory", "Available MBytes", null);
        PerformanceCounter mem2 = new PerformanceCounter("Memory", "Committed Bytes", null);
        PerformanceCounter mem3 = new PerformanceCounter("Memory", "Commit Limit", null);
        PerformanceCounter mem4 = new PerformanceCounter("Memory", "Pool Paged Bytes", null);
        PerformanceCounter mem5 = new PerformanceCounter("Memory", "Pool Nonpaged Bytes", null);
        PerformanceCounter mem6 = new PerformanceCounter("Memory", "Cache Bytes", null);


        //HDD
        PerformanceCounter hddpr = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");
        PerformanceCounter hddpr1 = new PerformanceCounter("PhysicalDisk", "Avg. Disk sec/Write", "_Total");
        PerformanceCounter hddpr2 = new PerformanceCounter("PhysicalDisk", "Avg. Disk sec/Read", "_Total");
        PerformanceCounter hddpr3 = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "_Total");
        PerformanceCounter hddpr4 = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "_Total");
        PerformanceCounter hddpr5 = new PerformanceCounter("PhysicalDisk", "Avg. Disk Queue Length", "_Total");
        PerformanceCounter hddpr6 = new PerformanceCounter("Paging File", "% Usage", "_Total");


        //Other
        PerformanceCounter ot = new PerformanceCounter("Process", "Handle Count", "_Total");
        PerformanceCounter ot1 = new PerformanceCounter("Process", "Thread Count", "_Total");
        PerformanceCounter ot2 = new PerformanceCounter("System", "Context Switches/sec", null);
        PerformanceCounter ot3 = new PerformanceCounter("System", "System Calls/sec", null);
        PerformanceCounter ot4 = new PerformanceCounter("System", "Processor Queue Length", null);

        bool chmini = false;
        public MainWindow()
        {

            InitializeComponent();

            int i = 0;
            int c = 0;
            int r = 0;
            this.Opacity = slider1.Value;

            while (i < Environment.ProcessorCount)
            {
                if (i % 2 == 0 && i !=0)
                {
                    r++;
                }
                if (i % 2 != 0 && i != 0)
                {
                    c++;

                }
                
                RowDefinition ii= new RowDefinition();
                grid1.RowDefinitions.Add(ii);

                Label label = new Label();
                label.Content = "C"+i+" - 100" + "%";
                label.Foreground = Brushes.White;
                label.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                label.HorizontalAlignment = HorizontalAlignment.Stretch;
                label.Name = "lbcpucore"+i;

                Grid.SetColumn(label, c);
                Grid.SetRow(label, r);
                grid1.Children.Add(label);

                i++;
                c = 0;
            }


            Thread thread1 = new Thread(DoWork);
            thread1.Start();
            Thread thread2 = new Thread(DoWork2);
            thread2.Start();
            Thread thread3 = new Thread(DoWork3);
            thread3.Start();
        }

        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        public async void DoWork3() {
            string pubIp ="";
            //LAN
            IPAddress[] addrList = Dns.GetHostByName(Dns.GetHostName()).AddressList;
            try
            {
                pubIp = new System.Net.WebClient().DownloadString("https://api.ipify.org");
            }
            catch { }



            await Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                //LAN
                lblan1.Content = "Public IP - " + pubIp;
                try
                {
                    lblan2.Content = "Lan IP - " + addrList[0].ToString();
                }
                catch (Exception)
                {
                }
                try
                {
                    lblan3.Content = "Lan IP - " + addrList[1].ToString();
                }
                catch (Exception)
                {
                }
            }));

            PerformanceCounterCategory performanceCounterCategory = new PerformanceCounterCategory("Network Interface");
            string instance = performanceCounterCategory.GetInstanceNames()[0]; // 1st NIC !
            PerformanceCounter performanceCounterSent = new PerformanceCounter("Network Interface", "Bytes Sent/sec", instance);
            PerformanceCounter performanceCounterReceived = new PerformanceCounter("Network Interface", "Bytes Received/sec", instance);

            while(true)
            {
                await Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    try
                    {
                        double sent = Math.Round(performanceCounterSent.NextValue() / 1024, 2);

                        if (sent > 0) { lblan4.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0));} 
                        else { lblan4.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF)); }

                        if (sent > 999) { lblan4.Content = "Sent(byte) - " + Math.Round(sent/1024,2) + " Mb/s"; }
                        else { lblan4.Content = "Sent(byte) - " + sent + " Kb/s"; }

                        
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        double rev = Math.Round(performanceCounterReceived.NextValue() / 1024, 2);

                        if (rev> 0){ lblan5.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0));
                        }else { lblan5.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF)); }


                        if (rev > 999) { lblan5.Content = "Received(byte) - " + Math.Round(rev / 1024,2) + " Mb/s"; }
                        else { lblan5.Content = "Received(byte) - " + rev + " Kb/s"; }
                        
                    }
                    catch (Exception)
                    {

                    }
                    int Desc;
                        string str = InternetGetConnectedState(out Desc, 0).ToString();
                        if (str == "False")
                        {
                            lblan1.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0, 0));
                        }
                        else
                        {
                            lblan1.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0, 0xFF, 0));
                        }
 

                }));
                Thread.Sleep(1000);
               
            }
        }

        public async void DoWork2()
        {
            await Dispatcher.BeginInvoke(new ThreadStart(delegate
            { 
                    Application.Current.MainWindow.Top = 0;
                    Application.Current.MainWindow.Left = SystemParameters.PrimaryScreenWidth - this.Width;

            }));


        }

        public async void DoWork()
            {
            ulong TotalRam = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / 1024 / 1024;
            while (true)
            {
                await Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    //CPU 5
                    Lbcpu1.Content = "Processor Time - " + Math.Round(cpuproc.NextValue()) + "%";
                    Lbcpu2.Content = "Privileged Time - " + Math.Round(cpuproc1.NextValue()) + "%";
                    //cpu2
                    Lbcpu3.Content = "User Time - " + Math.Round(cpuproc2.NextValue()) + "%";
                    Lbcpu4.Content = "Interrupt Time - " + Math.Round(cpuproc3.NextValue()) + "%";
                    Lbcpu5.Content = "DPC Time - " + Math.Round(cpuproc4.NextValue()) + "%";
                    Lbcpu6.Content = "Handle Count - " + Math.Round(ot.NextValue());
                    Lbcpu7.Content = "Thread Count - " + Math.Round(ot1.NextValue());
                    LbcpuCore.Content = "Core - " + Environment.ProcessorCount + " (in developing)";


                    //MEM  7

                    Lbmem1.Content = "Committed Bytes In Use - " + Math.Round(mem.NextValue()) + "%";

                    double usM = Math.Round((Convert.ToInt32(TotalRam) - Math.Round(mem1.NextValue())) / 1024, 2);
                    decimal allM = Math.Round(Convert.ToDecimal(TotalRam) / 1024);
                    double porUsM = Math.Round(usM * 100 / Convert.ToInt32(allM), 2);


                    if (porUsM > 50 && porUsM < 90)
                    {
                        Lbmem2.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0));
                        Lbmem3.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0));
                    }
                    else if (porUsM >= 90)
                    {
                        Lbmem2.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0, 0));
                        Lbmem3.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0, 0));
                    }
                    else
                    {
                        Lbmem2.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                        Lbmem3.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                    }

                    Lbmem2.Content = "Used memory - " + porUsM + "%";

                    Lbmem3.Content = "Used memory - " + usM + "/" + allM + " Gb";
                    Lbmem4.Content = "Available - " + Math.Round(mem1.NextValue() / 1024, 2) + " Gb";
                    Lbmem5.Content = "Committed - " + Math.Round(mem2.NextValue() / 10024 / 10024) + " Mb";
                    Lbmem6.Content = "Commit Limit - " + Math.Round(mem3.NextValue() / 1024 / 1024) + " Mb";
                    Lbmem7.Content = "Pool Paged Bytes - " + Math.Round(mem4.NextValue() / 1024 / 1024) + " Mb";
                    Lbmem8.Content = "Pool Nonpaged Bytes- " + Math.Round(mem5.NextValue() / 1024 / 1024) + " Mb";
                    Lbmem9.Content = "Cache Bytes - " + Math.Round(mem6.NextValue() / 1024 / 1024) + " Mb";

                    //HDD 7
                    Lbhdd1.Content = "Disk Time - " + Math.Round(Convert.ToDecimal(hddpr.NextValue()), 2) + "%"; 
                    Lbhdd2.Content = "Avg. Disk sec/Write - " +  Math.Round(hddpr1.NextValue(),5); 
                    Lbhdd3.Content = "Avg. Disk sec/Read - " + Math.Round(hddpr2.NextValue(),5);
                    Lbhdd4.Content = "Disk Write - " + Math.Round(Convert.ToDecimal(hddpr3.NextValue() / 1024 / 1024), 2)   + " Mb/sec ";
                    Lbhdd5.Content = "Disk Read - " + Math.Round(Convert.ToDecimal(hddpr4.NextValue() /1024/1024), 2) + " Mb/sec ";                    
                    Lbhdd6.Content = "Avg. Disk Queue Length - " + Math.Round(hddpr5.NextValue()) + "?";
                    Lbhdd6.Content = "Paging File - " + Math.Round(hddpr6.NextValue()) + "%";

                    //Other 5

                    lbot1.Content = "Context Switches/sec - " + Math.Round(ot2.NextValue()); 
                    lbot2.Content = "System Calls/sec - " + Math.Round(ot3.NextValue()); 
                    lbot3.Content = "Processor Queue Length - " + Math.Round(ot4.NextValue());
                }));
                Thread.Sleep(1000);

            }
        }
        bool _pin = false;
        private void Button_pin(object sender, RoutedEventArgs e)
        {
            _pin = !_pin;

            if (_pin == false)
            {
                btnpin.Content = "pin";
                this.Topmost = _pin;
            }
            else {
                btnpin.Content = "unpin";
                this.Topmost = _pin;
            }

        }

        private void btnclose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            mini fmini = new mini();
            fmini.Close();
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.Opacity = slider1.Value;
        }

        private void btnmini_Click(object sender, RoutedEventArgs e)
        {
            mini fmini = new mini();
            if (chmini == false) {
                fmini.Show();
                chmini = true;
            }else if (chmini == true){
                fmini.Hide();
                chmini = false;
            }
            

        }
    }

}

