import 'package:flutter/material.dart';
import 'homepage.dart';


// import 'package:flutter/services.dart';
// import 'package:csv/csv.dart';
// import 'dart:convert';

// Beginning of the App, Main method that calls runApp function and passes MyApp
void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return const MaterialApp(
      debugShowCheckedModeBanner: false,
      home: HomePage(),
    );
  }
}