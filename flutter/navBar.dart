import 'package:flutter/material.dart';

void main() {
  runApp(const MyApp());
}

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
          title: Text('Hello World'),
          centerTitle: true,
          backgroundColor: Colors.blueGrey,
        ),
        bottomNavigationBar: NavigationBar(
          destinations: [
            NavigationDestination(icon: Icon(Icons.home), label: 'home'),
            NavigationDestination(
              icon: Icon(Icons.contact_page),
              label: 'contact',
            ),
            NavigationDestination(
              icon: Icon(Icons.photo_album_outlined),
              label: 'about',
            ),
          ],
          selectedIndex: 1,
          onDestinationSelected: (int value) {
            print(value);
            if (value == 0) {
              print('home');
            } else if (value == 1) {
              print('contact');
            } else {
              print('about');
            }
          },
        ),
      ),
    );
  }
}
