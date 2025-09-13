import 'package:flutter/material.dart';
import 'package:flutter1/variables.dart';

class NavBar extends StatelessWidget {
  const NavBar({super.key});

  @override
  Widget build(BuildContext context) {
    return ValueListenableBuilder(
      valueListenable: selectedIndexNotifier,
      builder: (context, intVariable, child) {
        return NavigationBar(
          destinations: [
            NavigationDestination(icon: Icon(Icons.home), label: 'home'),
            NavigationDestination(icon: Icon(Icons.save), label: 'save'),
            FloatingActionButton(
              onPressed: () {
                brightnessNotifier.value = !brightnessNotifier.value;
              },
              child: ValueListenableBuilder(
                valueListenable: brightnessNotifier,
                builder: (context, isDarkMode, child) {
                  return isDarkMode
                      ? const Icon(Icons.light_mode)
                      : const Icon(Icons.dark_mode);
                },
              ),
            ),
          ],
          onDestinationSelected: (int num) {
            selectedIndexNotifier.value = num;
          },
          selectedIndex: intVariable,
        );
      },
    );
  }
}
