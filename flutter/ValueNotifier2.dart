import 'package:flutter/material.dart';

List<String> pageName = ['Settings', 'Profile'];
ValueNotifier<int> selectedBarNo = ValueNotifier<int>(0);

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
      home: ValueListenableBuilder(
        valueListenable: selectedBarNo,
        builder: (context, value, child) {
          return Scaffold(
            appBar: AppBar(
              title: Center(child: Text('Hello')),
              backgroundColor: Colors.blueGrey,
            ),
            body: () {
              if (value == 0) {
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
                NavigationDestination(
                  icon: Icon(Icons.person),
                  label: 'profile',
                ),
              ],
              onDestinationSelected: (int num) {
                selectedBarNo.value = num;
              },
              selectedIndex: value,
            ),
          );
        },
      ),
    );
  }
}
