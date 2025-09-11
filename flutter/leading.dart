import 'package:flutter/material.dart';

void main() {
  runApp(const MyApp());
}

//stateless
//material app
//scaffold
class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(
          seedColor: Colors.amber,
          brightness: Brightness.dark,
        ),
      ),
      home: Scaffold(
        appBar: AppBar(
          title: Text(
            'Hello World',
            style: TextStyle(
              fontSize: 40,
              fontStyle: FontStyle.italic,
              color: Colors.deepOrangeAccent,
            ),
          ),
          leading: Icon(Icons.login, color: Colors.purpleAccent, size: 40),
          centerTitle: true,
          backgroundColor: Colors.blueGrey,
        ),
      ),
    );
  }
}
