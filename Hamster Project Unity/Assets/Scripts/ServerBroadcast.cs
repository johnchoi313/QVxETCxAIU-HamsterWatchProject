using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;

//---HOW IT WORKS:---//

//1. Server starts and is listening.
//2. Client starts. Sends broadcast with own IP to 255.255.255.255 (all), to find server IP.
//3. Server picks message up, returns own IP to client. Maps client IP to player number.
//4. Client gets server IP - is connected!
//5. Client now spams control data to server with own IP tag.
//6. Server occasionally send message back??? (Backfeed.)
public class ServerBroadcast : MonoBehaviour {

    public string serverIP = "";
    
    public UDPReceiver allClients;
    public UDPSender serverBroadcaster;
    
    public List<RemoteIP> remoteIPs;
    public float clientReceivedTime = 3.0f;

    public float serverBroadcastTime = 3.0f;
    private float serverBroadcastTimer = 3.0f;
    
    public HamsterController[] hamsters;
    public HamsterTextures hamsterTextures;

    public AwesomeCharts.BreakChartController breakChart;
    public List<BreakDataFull> breakDataFulls;

    private string path;

    void Start() {
        serverIP = LocalIPAddress();
        remoteIPs = new List<RemoteIP>(); 

        //Load all breakDatas from file
        path = Application.streamingAssetsPath + "/BreakData.csv";
        breakDataFulls = new List<BreakDataFull>();
        LoadCSVBreakTable();
    }

    public void LoadCSVBreakTable() {
        Debug.Log("Load: " + path);
        StreamReader reader = new StreamReader(path); 
        string encodedString = reader.ReadToEnd().ToLower();
        reader.Close();
        string[][] table = CsvParser2.Parse(encodedString);    
        //Add each line of CSV File
        if(table.Length > 1);
        for(int i = 1; i < table.Length; i++) {
            string name = table[i][0];
            string date = table[i][1];
            int day = 0; if(!int.TryParse(table[i][2], out day)) { Debug.LogWarning("Table ["+i+"][2] must be int!"); }
            int hour = 0; if(!int.TryParse(table[i][3], out hour)) { Debug.LogWarning("Table ["+i+"][3] must be int!"); }
            int min = 0; if(!int.TryParse(table[i][4], out min)) { Debug.LogWarning("Table ["+i+"][4] must be int!"); }    
            //Add to Break data Full List
            breakDataFulls.Add(new BreakDataFull(name, date, day, hour, min));
            //Add to Break Chart Graph
            breakChart.AddBreakData(name, day, hour, min);
        }
    }
    public void SaveCSVBreakTable(string name, string date, int day, int hour, int min) {
        string CSVString = "";
        CSVString += name + ";";
        CSVString += date + ";";
        CSVString += day + ";";
        CSVString += hour + ";";
        CSVString += min + ";";
        //Save Palette to file
        StreamWriter writer = new StreamWriter(path, true); //true to append, false to overwrite
        writer.WriteLine(CSVString);
        writer.Close();
    }

    void Update() {
        //Broadcast IP address to everyone in network.
        if(serverBroadcastTimer > 0) { serverBroadcastTimer -= Time.deltaTime; } 
        else {
            serverBroadcastTimer = serverBroadcastTime; 
            serverBroadcaster.sendMessage(serverIP);    
            Debug.Log("Sent server broadcast.");
        }
        //Find data.
        string clientMessage = allClients.getMessage();
        if(clientMessage != null) {
            try {
                HamsterData hamsterData = JsonUtility.FromJson<HamsterData>(clientMessage);
                bool matchedIP = false;
                //Check if hamster is already matched, and if so, control hamster
                for(int i = 0; i < remoteIPs.Count; i++) {
                    if(hamsterData.IP == remoteIPs[i].IP && hamsterData.k == remoteIPs[i].key) {
                        if(0 <= remoteIPs[i].hamsterIndex && remoteIPs[i].hamsterIndex < hamsters.Length) {
                            hamsters[remoteIPs[i].hamsterIndex].setHamsterData(hamsterData); 
                            hamsterTextures.setFaceTexture( remoteIPs[i].hamsterIndex, hamsterData.f); 
                            hamsterTextures.setBodyTexture( remoteIPs[i].hamsterIndex, hamsterData.b); 

                            //1. Apply breakDatas from hamsterData to graph. Check and compare with name, day/hour/minute to make sure we don't get duplicates.
                            foreach(BreakData bd in hamsterData.bd) {
                                //Search everything in existing break data full list
                                bool duplicate = false;
                                foreach(BreakDataFull bdf in breakDataFulls) {
                                    if(bdf.name.ToLower().Equals(hamsterData.n.ToLower()) && bdf.day == bd.d && bdf.hour == bd.h && bdf.min ==  bd.m) {
                                        duplicate = true; break;
                                    } 
                                }
                                if(!duplicate) {
                                    //Show debug data
                                    Debug.Log("Added! " + bd.d + ":" + bd.h + ":" + bd.m);    
                                    //Add to list
                                    string date = System.DateTime.Now.ToString("MM/dd/yyyy");
                                    breakDataFulls.Add(new BreakDataFull(hamsterData.n, date, bd.d, bd.h, bd.m));
                                    //Add to graph
                                    breakChart.AddBreakData(hamsterData.n, bd.d, bd.h, bd.m);
                                    //Add to file 
                                    SaveCSVBreakTable(hamsterData.n, date, bd.d, bd.h, bd.m);
                                    //(NOTE: BREAKS ARE CLEARED FROM THE WATCH EACH DAY!)
                                }
                            }
                        }
                        remoteIPs[i].clientReceivedTimer = clientReceivedTime;
                        matchedIP = true;
                        break;
                    }   
                }
                //If hamster is not matched yet:
                if(!matchedIP) { 
                    //find available hamster
                    int availableIndex = -1;
                    for(int h = 0; h < hamsters.Length; h++) {
                        if(hamsters[h].available) {
                            hamsters[h].setAvailable(false);
                            availableIndex = h;
                            break;
                        }
                    }
                    //create new control assignment if available hamster found
                    if(availableIndex > -1) {
                        remoteIPs.Add(new RemoteIP(hamsterData.IP, hamsterData.k, availableIndex)); 
                        Debug.Log("Available hamster found and connected!");
                    } else {
                        Debug.LogWarning("No available hamsters found. Not connected.");    
                    }
                }
            } catch {
                Debug.LogWarning("Failed to parse JSON hamster message!");
            }
        }

        //If any hamster gets disconnected, remove hamster
        for(int i = 0; i < remoteIPs.Count; i++) {
            if(remoteIPs[i].clientReceivedTimer > 0) { remoteIPs[i].clientReceivedTimer -= Time.deltaTime; } 
            else { 
                if(hamsters[remoteIPs[i].hamsterIndex].available == false) {
                    hamsters[remoteIPs[i].hamsterIndex].setAvailable(true);
                    remoteIPs.RemoveAt(i); 
                    Debug.Log("Hamster disconnected.");
                }
            }
        }
    }

    //Helper Function gets local IP Address
    public string LocalIPAddress() {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList) {
            if (ip.AddressFamily == AddressFamily.InterNetwork) { localIP = ip.ToString(); break; }
        }
        return localIP;
    }
}

[System.Serializable]
public class RemoteIP {
    public string IP = "";
    public int key = 0;
    public int hamsterIndex = -1;
    public float clientReceivedTimer = 3.0f;
    public RemoteIP(string ip, int k, int index) { IP = ip; key = k; hamsterIndex = index; }
}

[System.Serializable]
public class BreakDataFull {
  public string name = ""; 
  public string date = "";

  public int day = 0; //Day (Mon-Fri)
  public int hour = 0; //Hour
  public int min = 0; //Min

  public BreakDataFull(string Name, string Date, int D, int H, int M) { 
    name = Name; date = Date; day = D; hour = H; min = M; 
  }
}