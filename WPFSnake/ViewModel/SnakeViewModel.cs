using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFSnake.Model;
using WPFSnake.Persistence;

namespace WPFSnake.ViewModel
{
    class SnakeViewModel :ViewModelBase
    {
        private SnakeModel _model; // modell
        private SaveEntry _selectedGame;
        private String _newName = String.Empty;

        //public DelegateCommand NewGameCommand { get; private set; }

        public DelegateCommand LoadGameOpenCommand { get; private set; }
        public DelegateCommand LoadGameCloseCommand { get; private set; }
        public DelegateCommand SaveGameOpenCommand { get; private set; }
        public DelegateCommand SaveGameCloseCommand { get; private set; }
        public DelegateCommand Control { get; private set; }
        public DelegateCommand NewGame { get; private set; }

        public ObservableCollection<ViewField> Fields { get; set; }
        public ObservableCollection<SaveEntry> Games { get; set; }

        public event EventHandler LoadGameOpen;
        public event EventHandler<String> LoadGameClose;
        public event EventHandler SaveGameOpen;
        public event EventHandler<String> SaveGameClose;

        public String NewName
        {
            get { return _newName; }
            set
            {
                _newName = value;

                OnPropertyChanged();
                SaveGameCloseCommand.RaiseCanExecuteChanged();
            }
        }

        public SaveEntry SelectedGame
        {
            get { return _selectedGame; }
            set
            {
                _selectedGame = value;
                if (_selectedGame != null)
                    NewName = String.Copy(_selectedGame.Name);

                OnPropertyChanged();
                LoadGameCloseCommand.RaiseCanExecuteChanged();
                SaveGameCloseCommand.RaiseCanExecuteChanged();
            }
        }

        


        public SnakeViewModel(SnakeModel model)
        {
            _model = model;

            _model.GameOver += Model_GameOver;
            _model.SnakeMoved += RefreshTable;

            Fields = new ObservableCollection<ViewField>();
            NewGame = new DelegateCommand(OnNewGame);
            Control = new DelegateCommand(KeyHandler);

            LoadGameOpenCommand = new DelegateCommand(param =>
            {
                Games = new ObservableCollection<SaveEntry>(_model.List());
                OnLoadGameOpen();
            });
            LoadGameCloseCommand = new DelegateCommand(
                param => SelectedGame != null,
                param => {
                    OnLoadGameClose(SelectedGame.Name);
                });
            SaveGameOpenCommand = new DelegateCommand(param =>
            {
                Games = new ObservableCollection<SaveEntry>(_model.List());
                OnSaveGameOpen();
            });
            SaveGameCloseCommand = new DelegateCommand(
                param => NewName.Length > 0,
                param => {
                    OnSaveGameClose(NewName);
                });

            //MakeGameTable();
        }

        public void MakeGameTable()
        {
            Fields.Clear();
            for(int i = 0; i < _model.MapSize;++i)
            {
                for (int k = 0; k < _model.MapSize; ++k)
                {
                    ViewField f = new ViewField
                    {
                        X = k * 30,
                        Y = i * 30,
                        Color = GetColor(_model.GetTile(k, i))
                    };
                    if (_model.Snake.Contains(new WPFSnake.Model.Point(k, i)))
                        f.Color = "Green";
                    Fields.Add(f);
                }
            }
        }
        /*
        private void RefreshGameTable()
        {
            Fields.Clear();
            for (int i = 0; i < _model.MapSize; ++i)
            {
                for (int k = 0; k < _model.MapSize; ++k)
                {
                    Fields.Add(new ViewField {
                        X = k,
                        Y = i,
                        Color = ( _model.Snake.Contains(new WPFSnake.Model.Point(k, i)) ? "Green": GetColor(_model.GetTile(k,i)) )
                    });
                }
            }

        }
        */
        private void KeyHandler(object sender)
        {
            string direction = Convert.ToString(sender);
            if (direction == "up")
            {
                _model.Turn(Direction.up);
            }
            if (direction == "down")
            {
                _model.Turn(Direction.down);
            }
            if (direction == "left")
            {
                _model.Turn(Direction.left);
            }
            if (direction == "right")
            {
                _model.Turn(Direction.right);
            }
            if (direction == "pause")
            {
                _model.IsPaused = !_model.IsPaused;
            }
        }

        private void Model_GameOver(object sender, GameOverEventArgs e)
        {
            _model.IsPaused = true;
            MessageBox.Show(String.Format("You scored {0} points", e.Score), "A kigyód kigyozott");
        }
        /*
        private void Model_GameCreated(object sender, EventArgs e)
        {
            MakeGameTable();
        }
        */
        private void RefreshTable(object sender, EventArgs e)
        {
            foreach(var field in Fields)
            {
                field.Color = _model.Snake.Contains(new WPFSnake.Model.Point(field.X/30, field.Y/30))? "Green" :GetColor(_model.GetTile(field.X/30, field.Y/30));
            }
        }

        private void OnNewGame(object sender)
        {
            Int32 size = Convert.ToInt32(sender);
            _model.NewGame(String.Format("src/map{0}.txt", size));
            MakeGameTable();
        }

        private void OnLoadGameOpen()
        {
            if (LoadGameOpen != null)
                LoadGameOpen(this, EventArgs.Empty);
        }
        private void OnLoadGameClose(String name)
        {
            if (LoadGameClose != null)
            {
                LoadGameClose(this, name);
                OnPropertyChanged();
                MakeGameTable();
            }
        }
        private void OnSaveGameOpen()
        {
            if (SaveGameOpen != null)
                SaveGameOpen(this, EventArgs.Empty);
        }
        private void OnSaveGameClose(String name)
        {
            if (SaveGameClose != null)
                SaveGameClose(this, name);
        }

        private string GetColor(Tile t)
        {
            if (t == Tile.apple)
                return "Red";
            else if (t == Tile.ground)
                return "White";
            else if (t == Tile.wall)
                return "Black";
            else return "Yellow";
        }
    }
}
