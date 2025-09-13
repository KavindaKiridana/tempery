import 'package:flutter/material.dart';
import 'package:flutter1/home.dart';
import 'package:flutter1/save.dart';
import 'package:flutter1/nav_bar.dart';
import 'package:flutter1/variables.dart';

List<Widget> pages = [Home(), Save()];

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});
  @override
  Widget build(BuildContext context) {
    return ValueListenableBuilder(
      valueListenable: brightnessNotifier,
      builder: (context, isDarkMode, child) {
        return MaterialApp(
          debugShowCheckedModeBanner: false,
          theme: ThemeData(
            brightness: isDarkMode ? Brightness.dark : Brightness.light,
          ),
          home: Scaffold(
            appBar: AppBar(
              title: Center(child: Text('hello')),
              backgroundColor: Colors.blueGrey,
            ),
            body: ValueListenableBuilder(
              valueListenable: selectedIndexNotifier,
              builder: (context, value, child) {
                return pages.elementAt(value);
              },
            ),
            bottomNavigationBar: NavBar(),
          ),
        );
      },
    );
  }
}
