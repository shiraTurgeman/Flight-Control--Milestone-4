
# Flight-Control--Milestone-4


#### Creators: 
##### Shira Turgeman & Noa Elishmereni
#####  [our GitHub](https://github.com/noaElish/Flight-Control--Milestone-4)

### **Basic information**
* Purpose of this extercise-
we created a control system to monitor active flights, as well as adding new flights. the programm works in a way so that it shows only the active flights at the moment. the system reset every few second, in order to be as accurate as possible. 
in addition, our system is in sync with other systems as well. as tou can see, the system follows internal flights- the ones we inserted, as well as external flights from external flights. 

 <p align="center">
 <img src=".\fc-image.png" width="500" height="300">
</p>


### **How does it work?**
If you wish to insert some flight- there are two main ways to do that:
* External Flights- you can log as a new system to our system, using the "Advanced REST client" app, following these steps:
  * enter the requsded url in the url box.
  * use the json format for external flights as shown in in the picture below. 
  * choose the "Post" option.
   <p align="center">
   <img src=".\REST.png" width="500" height="260">
   </p>
  * to add external flights to the server, copy the external servers url to the "Resquest URL", and add the json flight details in the box below, as shown in    the picture:
   <p align="center">
   <img src=".\rest2.png" width="600" height="260">
   </p>
 
* Internal Flights
  * you can insert new Json file with the flight detailes and insert it in the "upload new flight" button.
    notice the time of the flight in the json should be 3 hours preior to the current time.
    
#### **Json format for Internal flights**
{
 "passengers": 216,
 "company_name": "SwissAir",
 "initial_location": {
 "longitude": 33.244,
 "latitude": 31.12,
 "date_time": "2020-12-26T23:56:21Z"
 },
 "segments": [
 {
 "longitude": 33.234,
 "latitude": 31.18,
 "timespan_seconds": 650
 },
 /... more segments.../
 ]
}
#### **Json format for External Servers**
{
 "ServerId": "[SERVER_ID]",
 "ServerURL": "www.server.com"
}

### **How to use**
1. when downloading the code from GitHub, a new zip directory will appear. 
please open the zip and Extract the code to a new directory that will be used from now- called "FlightControl".
2. comprass all the file into a zip file called "FlightControl"- all except the bat.
3. place the bat in the same directory in the same level as the zip called "FlightControl".
4. make sure to leave the bat and zip only in the file, and nothing else but that. 
5. click the bat- and a new window will appear running the files. 
6. when the process in over, a new window will appear with information regarding the programm. copy the url from the second line as shown in the picture bellow-

 <p align="center">
 <img src=".\compile.png" width="700" height="100">
</p>

7. copy the url and open it.

