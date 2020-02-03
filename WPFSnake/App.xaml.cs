using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPFSnake.Model;
using WPFSnake.ViewModel;
using WPFSnake.View;
using WPFSnake.Persistence;

namespace WPFSnake
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private SnakeModel _model;
        private SnakeViewModel _viewmodel;
        private MainWindow _view;
        private LoadGameWindow _loadwindow;
        private SaveGameWindow _savewindow;

        public App()
        {
            Startup += App_StartUp;
        }

        private void App_StartUp(object sender, StartupEventArgs e)
        {

            DbPersistence _dataAccess = new DbPersistence("name=SnakeModel");
            _model = new SnakeModel(_dataAccess);

            _viewmodel = new SnakeViewModel(_model);


            _viewmodel.LoadGameOpen += new EventHandler(ViewModel_LoadGameOpen);
            _viewmodel.LoadGameClose += new EventHandler<String>(ViewModel_LoadGameClose);
            _viewmodel.SaveGameOpen += new EventHandler(SaveGameOpen);
            _viewmodel.SaveGameClose += new EventHandler<String>(SaveGameClose);

            _view = new MainWindow();
            _view.DataContext = _viewmodel;

            _view.Show();
            
        }
        private void SaveGameOpen(object sender, EventArgs e)
        {
            _viewmodel.SelectedGame = null;
            _viewmodel.NewName = String.Empty;
            _model.IsPaused = true;
            _savewindow = new SaveGameWindow();
            _savewindow.DataContext = _viewmodel;
            _savewindow.ShowDialog();
        }

        private async void SaveGameClose(object sender, string e)
        {
            if (e != null)
            {
                try
                {
                    var games = _model.List();
                    if (games.All(g => g.Name != e) ||
                        MessageBox.Show("Biztos, hogy felülírja a meglévő mentést?", "Snake",
                            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        await _model.SaveGameState(e);
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            _model.IsPaused = false;
            _savewindow.Close();
        }

        private void ViewModel_LoadGameOpen(object sender, EventArgs e)
        {
            _viewmodel.SelectedGame = null;
            _model.IsPaused = true;
            _loadwindow = new LoadGameWindow();
            _loadwindow.DataContext = _viewmodel;
            _loadwindow.ShowDialog();

        }
        private async void ViewModel_LoadGameClose(object sender, string e)
        {
            if (e != null)
            {
                try
                {
                    await _model.LoadGameState(e);
                    _viewmodel.MakeGameTable();
                }
                catch
                {
                    MessageBox.Show("Játék betöltése sikertelen!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            _loadwindow.Close();
            //_viewmodel.MakeGameTable();
            _model.IsPaused = false;
        }

    }
}
