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
          leading: Icon(Icons.login),
          actions: [
            Text('data'),
            Icon(Icons.arrow_back),
            Icon(Icons.ac_unit_rounded),
            Icon(Icons.import_contacts),
          ],
          centerTitle: true,
          backgroundColor: Colors.blueGrey,
        ),
      ),
    );
  }
}
