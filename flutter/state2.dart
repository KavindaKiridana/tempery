import 'package:flutter/material.dart';

void main() {
  runApp(const MyWidget());
}

class MyWidget extends StatefulWidget {
  const MyWidget({super.key});

  @override
  State<MyWidget> createState() => _MyWidgetState();
}

class _MyWidgetState extends State<MyWidget> {
  int countIndex = 0;
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
      home: SafeArea(
        child: Scaffold(
          appBar: AppBar(
            title: Text('Hello World'),
            centerTitle: true,
            backgroundColor: Colors.blueGrey,
          ),
          body: () {
            if (countIndex == 0) {
              return Center(child: Text('one'));
            } else if (countIndex == 1) {
              return Center(child: Text('two'));
            } else {
              return Center(child: Text('three'));
            }
          }(),
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
            onDestinationSelected: (int value) {
              setState(() {
                countIndex = value;
              });
            },
            selectedIndex: countIndex,
          ),
        ),
      ),
    );
  }
}
