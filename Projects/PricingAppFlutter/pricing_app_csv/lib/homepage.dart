import 'package:csv/csv.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'dart:async';

const config = {
  "apiUrl": "10.0.2.2:8000",  // Connect to localhost for Android emulator
};

const config1 = {
  "apiUrl": "127.0.0.1:8000",  // Connect to localhost for non Android emulator
};

class HomePage extends StatefulWidget {
  const HomePage({super.key});

  @override
  _HomePageState createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> 
{

// --------------------------------------------------------------------------------------------------------------------

// Function to call the api that asynchronously connects to the MSSQL database

  //List<dynamic> _poData = [];
  String unpriced = '';

  Future <String> _fetchPOData() async 
  {
    var url = Uri.http(config1["apiUrl"]!, '/');
    final response = await http.get(url);
  
  if (response.statusCode == 200) 
  {
    // If the server returns a 200 OK response, parse the JSON.
    final List<dynamic> poData = json.decode(response.body);
    // _poData = poData;

    var inputModifier = int.parse(userInput);
      
      for(var p in poData)
      {
        bool inputCheck = p.containsValue(inputModifier); //PO Line Number
        bool inputCheck1 = p.containsValue(userInput1); // PO Number

        if(inputCheck1 == true && inputCheck == true)
        {
          unpriced = "Currently Unpriced";
          print("$userInput1 $userInput From PO Exists!"); 
          return unpriced;
        }    
      }
      unpriced = "Already Priced";
      print("$userInput1 $userInput PO Doesn't Exist!");  
      return unpriced;
  } 
  else 
  {
    // If the server returns an error response, throw an exception.
    throw Exception('Failed to load data.');
  }
  }

// --------------------------------------------------------------------------------------------------------------------

// Loading the CSV data from the local assets folder

  List<dynamic> _data = [];
  String exists = '';

  Future <String> _loadCSV() async {
    final rawData = await rootBundle.loadString("assets/Raw Material Bookings1.csv");
    List<dynamic> listData = const CsvToListConverter().convert(rawData);
      _data = listData;
      
      for(var i in _data)
      {
        bool inputCheck = i.contains(_selectedSupplier);
        bool inputCheck1 = i.contains(userInput1);
          
        if(inputCheck1 == true && inputCheck == true)
        {
          exists = "Combination Valid";
          print("$_selectedSupplier $userInput1 From CSV Exists!");
          return exists;
          
        }    
      }
      exists = "Combination NOT Valid";
      print("$_selectedSupplier $userInput1 From CSV Doesn't Exist!");
      return exists;
      
  }

// --------------------------------------------------------------------------------------------------------------------

  // This controller allows you to get what the user is typing
  final _textController = TextEditingController();
  final _textController1 = TextEditingController();

  // Use this variable to store user text input
  String userInput = '';
  String userInput1 = '';

  final List<String> _suppliers = ['Select a Supplier...', 'Alter Trading', 'Atlas Metals', 'Calbag Metals', 'Commercial Metal', 'Ecovery', 'Exeon', 'Geomet', 'GLE', 
                                  'Intrametco (Lakeside)', 'K&K', 'Merivs', 'Metal Processors', 'Metal X', 'Metro', 'Pacific Steel', 'Padnos', 'Radius', 'SA Recycling', 
                                  'Shine', 'Southwest', 'Tucson Iron & Metal', 'United Metals', 'Utah Metal'];
  String _selectedSupplier = 'Select a Supplier...';

  @override
  Widget build(BuildContext context){
    return Scaffold(
      body: Padding(
        padding: const EdgeInsets.all(20.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.end,
          mainAxisAlignment: MainAxisAlignment.center,
          children: [

            //Displaying some text
            Expanded(
              child: Container(
                child: const Center(
                  child: Text('Welcome to the ACC pricing app.\nEnter a PO # below to check if it\'s currently unpriced.', textAlign: TextAlign.center, style: TextStyle(fontSize: 15),),
                ),
              ),
            ),

            //Displaying some text
            Expanded(
              child: Container(
                child: Center(
                  child: 
                    Text('The PO # requested for a price check was:\n$_selectedSupplier $userInput1-$userInput', textAlign: TextAlign.center, style: const TextStyle(fontSize: 15),),
                ),
              ),
            ),

            //Displaying some text
            Expanded(
              child: Container(
                child: Center(
                  child: 
                    Text('Supplier / PO # Validation Result:\n$exists', textAlign: TextAlign.center, style: const TextStyle(fontSize: 15),),
                ),
              ),
            ),

            //Displaying some text
            Expanded(
              child: Container(
                child: Center(
                  child: 
                    Text('Pricing Request Result:\n$unpriced', textAlign: TextAlign.center, style: const TextStyle(fontSize: 15),),
                ),
              ),
            ),

            // Drop Down List for User Selection
            DropdownButton(
            value: _selectedSupplier,
            onChanged: (newValue) {
              setState(() {
                _selectedSupplier = newValue!;
              });
            },
            items: _suppliers.map((location) {
              return DropdownMenuItem(
                value: location,
                child: Text(location),
              );
            }).toList(),
            ),

            // Text Field for User Input - PO Number
            TextField(
              controller: _textController1,
              decoration: InputDecoration(
                hintText: 'Input PO Number...',
                border: const OutlineInputBorder(),
                suffixIcon: IconButton(
                  onPressed: () {
                    _textController1.clear();
                  },
                  icon: const Icon(Icons.clear),
                ),
              ),
            ),

            // Text Field for User Input - PO Line Number
            TextField(
              controller: _textController,
              decoration: InputDecoration(
                hintText: 'Input PO Line Number...',
                border: const OutlineInputBorder(),
                suffixIcon: IconButton(
                  onPressed: () {
                    _textController.clear();
                  },
                  icon: const Icon(Icons.clear),
                ),
              ),
            ),

            // Submit Button to send request - Supplier / PO # Combination
            MaterialButton(
              onPressed: () {
               _loadCSV();

              // Updates the user input string variable to get the new user input when the button is pressed.
              setState((){
              userInput = _textController.text;
              });
              },
              color: Colors.blue,
              child: const Text('Check Supplier / PO #', style: TextStyle(color: Colors.white))
            ),

            // Submit Button to send request - PO # / Line # Combination
             MaterialButton(
              onPressed: () {
               _fetchPOData();

              // Updates the user input string variable to get the new user input when the button is pressed.
              setState((){
              userInput1 = _textController1.text;
                });
              },
              color: Colors.blue,
              child: const Text('Check PO # / Line #', style: TextStyle(color: Colors.white))
            )
          ]
        )
      ),
    );
  }
}