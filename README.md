# WPF Snake
 College Project : Snake with fancy
 
Technologies used: .NET ,C# ,Entity Framework
Architecture: MVVM

Overview:
  Played on N x N map, 3 maps are made, but you can make your own map, pretty fast
  Model moves the Snake, every some miliseconds
  View contains 3 xaml files -> they are doing their job displaying the game, load window, save window
  View Model has some delegate commands, based on this View displays the game
  App.xaml.cs connects everything 
  Persistence folder, has the Entity magic -> classes to store data in Database, and the DataBase itself
