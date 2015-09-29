// PoC Sniff OpenVPN configuration from STDIN and Management Port
// Created with Mono 
// Version 0 (PoC) // Nicolas GOLLET

/*
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>
*/

/*
 * This Proof of concept is used to grab "Fsecure Freedome" config to use it on Linux with OpenVPN
 * you must adapte config file to run fine :) Read OpenVPN manual for more information
 * 
 * Rename original openvpn.exe to openvpn2.exe
 * Copy openvpn.exe to Freedome OpenVPN directory 
 * Run VPN connection
 * Config file is saved in 	:  _PoC_ConfigFile.txt
 * and Passphrase in 		:  openvpnMgmtIn.log
 * 
 * Adapte config file and Have Fun :)
 * 
 */

using System;
using System.Diagnostics;
using System.Threading;
using System.Net;

namespace openvpn
{
	class MainClass
	{
		
		
	
		
		public static void Main (string[] args)
		{
			// OpenVPN Stdin & management port "sniffer"
            // PoC for Grab F-Secure Freedome VPN information
			string fileInName = "_PoC_ConfigFile.txt";
			System.IO.StreamWriter fileIn = new System.IO.StreamWriter(fileInName);
			int port = 0;
			
			string s;
            while ((s = Console.ReadLine()) != null) // null = EOL
            {
				// remplacement du port du mgmt
                if (s.StartsWith("management 127.0.0.1"))
                {
                  
                    //modifie le port de management
                    fileIn.WriteLine("management 127.0.0.1 1234");
                    fileIn.Flush();
					
                    // start new tcp wrapper thread
                    // recup du port de management réel
                    string[] managementrealport = s.Split(' ');
                    port = int.Parse(managementrealport[2]);

                }
                else
                {
                   
                    fileIn.WriteLine(s);
                    fileIn.Flush();
                }
            }
			
			MyThreadHandle threadHandle = new MyThreadHandle(port);
			Thread t = new Thread(new ThreadStart(threadHandle.ThreadLoop));
			t.Start();

			
			// demarrage de OpenVPN // renommé openvpn.exe en openvpn2.exe
			ProcessStartInfo processStartInfo;
            Process process;

            // Start real process

            processStartInfo = new ProcessStartInfo();
            processStartInfo.CreateNoWindow = false;
            processStartInfo.UseShellExecute = true;
			// paramettre pour openvpn2
            processStartInfo.Arguments = "--config _PoC_ConfigFile.txt";
            processStartInfo.FileName = "openvpn2.exe"; 


            process = new Process();
            process.StartInfo = processStartInfo;
            process.EnableRaisingEvents = true;
			
			process.Start();
			
			
			// attente
			
			while (true){
			
				if (process.HasExited == true){
					t.Abort();				
					break;
				}
				// restart portforwarder
				if (t.IsAlive == false){
					t.Start();
					
				}
				
			}
	

		}
	}
}
