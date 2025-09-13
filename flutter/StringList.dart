import 'package:flutter/material.dart';

List<String> pageName = ['Settings', 'Profile'];

void main() {
  runApp(const MyApp());
}

class MyApp extends StatefulWidget {
  const MyApp({super.key});

  @override
  State<MyApp> createState() => _MyAppState();
}

class _MyAppState extends State<MyApp> {
  int selectedBar = 0;
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      theme: ThemeData(brightness: Brightness.dark),
      home: Scaffold(
        appBar: AppBar(
          title: Center(child: Text('Hello')),
          backgroundColor: Colors.blueGrey,
        ),
        body: () {
          if (selectedBar == 0) {
            return Center(child: Text(pageName.elementAt(0)));
          } else {
            return Center(child: Text(pageName.elementAt(1)));
          }
        }(),
        bottomNavigationBar: NavigationBar(
          destinations: [
            NavigationDestination(
              icon: Icon(Icons.settings),
              label: 'settings',
            ),
            NavigationDestination(icon: Icon(Icons.person), label: 'profile'),
          ],
          onDestinationSelected: (int value) {
            setState(() {
              selectedBar = value;
            });
          },
          selectedIndex: selectedBar,
        ),
      ),
    );
  }
}
