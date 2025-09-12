import 'package:flutter/material.dart';

final counter = ValueNotifier<int>(0);

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      theme: ThemeData(brightness: Brightness.dark),
      home: Scaffold(
        body: ValueListenableBuilder<int>(
          valueListenable: counter,
          builder: (context, value, child) {
            return Text('Counter: $value');
          },
        ),
        floatingActionButton: FloatingActionButton(
          onPressed: () {
            counter.value++;
          },
          child: Icon(Icons.add),
        ),
      ),
    );
  }
}
