using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;

namespace ProjetWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Board m_board = new Board();
        Token m_selectedToken;
        List<TokenMove> m_selectedTokenPossibleMove;
        List<TokenMoveSequence> m_sequenceEngaged;

        bool m_playerTurn;
        Dictionary<Token, List<TokenMoveSequence>> m_playerPossibleMove;

        bool m_stopGame = false;

        public MainWindow()
        {
            InitializeComponent();
            FillBoardColor();
            DisplayBoard();
            
            // Lance la boucle de jeu
            m_playerTurn = false;
            Thread gameThread = new Thread(new ThreadStart(GameLoop));
            gameThread.Start();
        }

        /// <summary>
        /// Rempli les couleurs du terrain
        /// </summary>
        private void FillBoardColor()
        {
            for(int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if(i % 2 == 0 && j % 2 == 1 || i % 2 == 1 && j % 2 == 0)
                    {
                        SetRectangle(i, j, Colors.Khaki);
                    }
                }
            }
        }

        /// <summary>
        /// Change la couleur de la case
        /// </summary>
        /// <param name="x"> Position x de la case</param>
        /// <param name="y"> Position y de la case</param>
        /// <param name="color"> La couleur a changer </param>
        private void SetRectangle(int x, int y, Color color)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Fill = new SolidColorBrush(color);
            GridBoard.Children.Add(rectangle);
            Grid.SetColumn(rectangle, x);
            Grid.SetRow(rectangle, y);
        }

        /// <summary>
        /// Display board from the loop thread
        /// </summary>
        public void DisplayBoardFromThread() {
            Dispatcher.Invoke(() => DisplayBoard());
        }

        /// <summary>
        /// Creer une nouvelle image a integrer a l'application
        /// </summary>
        /// <param name="imageName">Chemin vers l'image</param>
        /// <returns> L'image créé </returns>
        private Image CreateImage(string imageName)
        {
            Image image = new Image();
            image.Stretch = Stretch.Fill;
            image.Visibility = Visibility.Collapsed;
            image.VerticalAlignment = VerticalAlignment.Center;
            image.Source = new BitmapImage(new Uri("Images/" + imageName, UriKind.Relative));

            return image;
        }

        void DisplayBoard()
        {
            ClearImages();
            for(int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if(m_board.Tokens[i,j] != null)
                    {
                        Image token = CreateImage(m_board.Tokens[i,j].image);
                        token.Visibility = Visibility.Visible;
                        GridBoard.Children.Add(token);
                        Grid.SetRow(token, i);
                        Grid.SetColumn(token, j);
                    }
                }
            }
        }
        private void ClearImages()
        {
            UIElement[] collection = new UIElement[GridBoard.Children.Count];
            GridBoard.Children.CopyTo(collection, 0);
            foreach (UIElement child in collection)
            {
                if(!(child is Rectangle))
                    GridBoard.Children.Remove(child);
            }
        }


        /// <summary>
        /// Le joueur a t'il gagne ? 
        /// </summary>
        /// <param name="player"> Le joueur a verifier</param>
        /// <returns>Vrai si il a gagne, faux sinon </returns>
        bool CheckWin(Token.TokenColor player)
        {
            switch (player)
            {
                case Token.TokenColor.White: return m_board.BlackCount == 0;
                case Token.TokenColor.Black: return m_board.WhiteCount == 0;
                default: break;
            }
            return false;
        }

        /// <summary>
        /// Change de tour
        /// </summary>
        /// <param name="current">Joueur qui a joue le dernier tour</param>
        /// <returns>Le joueur qui doit jouer</returns>
        Token.TokenColor SwitchPlayer(Token.TokenColor current)
        {
            switch (current)
            {
                case Token.TokenColor.White:
                    Dispatcher.Invoke(() => Player.Text = "Joueur actuel : Noir");
                    return Token.TokenColor.Black;
                case Token.TokenColor.Black:
                    Dispatcher.Invoke(() => Player.Text = "Joueur actuel : Blanc");
                    return Token.TokenColor.White;
                default: break;
            }
            return current;
        }


        void StartGame() {
            ReplayButton.Visibility = Visibility.Hidden;
            EndGameText.Visibility = Visibility.Hidden;
        }

        void EndGame(Token.TokenColor player) {
            string victory = "Victoire des blanc";
            if (player == Token.TokenColor.Black) victory = "Victoire des noirs";
            ReplayButton.Visibility = Visibility.Visible;
            EndGameText.Visibility = Visibility.Visible;
            EndGameText.Text = victory;
        }

        /// <summary>
        /// Boucle de Jeu
        /// </summary>
        void GameLoop()
        {
            Dispatcher.Invoke(() => StartGame());

            Token.TokenColor currentPlayer = Token.TokenColor.White;
            bool lastPlayerWon = false;
            CheckersSolver opponent = new CheckersSolver(this, m_board, 7);

            Dispatcher.Invoke(() => Player.Text = "Joueur actuel : Blanc");

            while (!lastPlayerWon && !m_stopGame)
            {
                switch (currentPlayer)
                {
                    case Token.TokenColor.White:
                        // On attend les instructions du joueur
                        m_playerTurn = true;
                        // Cette fonction est assez couteuse, il faudrait l'appeler un minimum de fois
                        m_playerPossibleMove = m_board.PrioritaryTokens(currentPlayer);
                        if (m_playerPossibleMove.Count == 0) lastPlayerWon = true;
                        m_sequenceEngaged = new List<TokenMoveSequence>();
                        while (m_playerTurn && !m_stopGame)
                        {
                            Thread.Sleep(0);
                        }
                        break;
                    case Token.TokenColor.Black:
                        // On attend les instructions de l'agent intelligent
                        lastPlayerWon = !opponent.ExecuteAMove();
                        // On attend les instructions du joueur
                        //m_playerTurn = true;
                        //// Cette fonction est assez couteuse, il faudrait l'appeler un minimum de fois
                        //m_playerPossibleMove = m_board.PrioritaryTokens(currentPlayer);
                        //if (m_playerPossibleMove.Count == 0)
                        //{
                        //    m_playerTurn = false;
                        //    break;
                        //}
                        //m_sequenceEngaged = new List<TokenMoveSequence>();
                        //while (m_playerTurn)
                        //{
                        //    Thread.Sleep(0);
                        //}

                        break;
                    default:
                        break;
                }

                if (lastPlayerWon) {
                    // C'est le joueur precedent qui a gagne
                    currentPlayer = SwitchPlayer(currentPlayer);
                    break;
                }

                // On verifie si apres c'est mouvement le joueur a gagné
                lastPlayerWon = CheckWin(currentPlayer);
                if (!lastPlayerWon) {
                    // C'est le tour du joueur suivant
                    currentPlayer = SwitchPlayer(currentPlayer);
                }
            }
            Dispatcher.Invoke(() => EndGame(currentPlayer));
        }

        class FindMove {
            Vector2 m_position;

            public FindMove(Vector2 position) {
                m_position = position;
            }

            public bool Match(TokenMove token) {
                return token.Position.Equals(m_position);
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (m_playerTurn) {
                var element = Mouse.DirectlyOver;
                if (element != null)
                {
                    // Si un pion est déjà selectionné 
                    if (m_selectedToken != null && m_playerPossibleMove.ContainsKey(m_selectedToken))
                    {
                        // On recupere la cellule selctionner et on recupere le mouvement associe
                        Vector2 selectedCell = new Vector2(Grid.GetColumn((UIElement)element), Grid.GetRow((UIElement)element));
                        FindMove move = new FindMove(selectedCell);
                        TokenMove tokenMove = m_selectedTokenPossibleMove.Find(move.Match);

                        // Si la cellule correspond a un deplacement l'effectuer 
                        if (tokenMove != null)
                        {
                            // Move the token and reset the colors of the board
                            ChangeColorOfCells(m_selectedTokenPossibleMove, Colors.Khaki);
                            if (m_selectedToken != null)
                            {
                                m_board.ExecuteTokenMove(m_selectedToken, tokenMove);
                                DisplayBoard();
                                //Calcul les captures possible restante
                                if (m_sequenceEngaged.Count > 0)
                                {
                                    List<TokenMoveSequence> toRemove = new List<TokenMoveSequence>();
                                    foreach (TokenMoveSequence sequence in m_sequenceEngaged)
                                    {
                                        TokenMove movePlayed = sequence.PlayMove();
                                        if (sequence.Empty() || !movePlayed.Equals(tokenMove)) toRemove.Add(sequence);
                                    }
                                    foreach (TokenMoveSequence sequence in toRemove)
                                    {
                                        m_sequenceEngaged.Remove(sequence);
                                    }
                                }
                                else
                                {
                                    foreach (TokenMoveSequence sequence in m_playerPossibleMove[m_selectedToken])
                                    {
                                        TokenMove movePlayed = sequence.PlayMove();
                                        if (!sequence.Empty() && movePlayed.Equals(tokenMove))
                                        {
                                            m_sequenceEngaged.Add(sequence);
                                        }
                                    }
                                }


                                // Fini le tour du joueur ou affiche les prochains mouvement possible
                                if (m_sequenceEngaged.Count == 0)
                                {
                                    if (m_selectedToken.Position.Y == 0)
                                    {
                                        m_board.Crown(m_selectedToken);
                                    }
                                    DisplayBoard();
                                    m_playerTurn = false;
                                    m_selectedToken = null;
                                    m_selectedTokenPossibleMove = null;
                                    m_sequenceEngaged = null;
                                }
                                else
                                {
                                    m_selectedTokenPossibleMove = ComputeCells(m_sequenceEngaged);
                                    ChangeColorOfCells(m_selectedTokenPossibleMove, Colors.CadetBlue);
                                }
                            }
                        }
                        // Sinon on deselectionne le pion
                        else if (m_sequenceEngaged.Count == 0){
                                if (m_selectedTokenPossibleMove == null) return;
                                // Reset selected token
                                ChangeColorOfCells(m_selectedTokenPossibleMove, Colors.Khaki);
                                m_selectedToken = null;
                        }
                        
                    }
                    else
                    {
                        // Si on a pas deja commence une sequence de mouvement
                        if (m_sequenceEngaged.Count == 0)
                        {
                            // Select the token and mark the possible destinations with a color
                            m_selectedToken = m_board.Tokens[Grid.GetRow((UIElement)element), Grid.GetColumn((UIElement)element)];
                            if (m_selectedToken != null && m_playerPossibleMove.ContainsKey(m_selectedToken))
                            {
                                m_selectedTokenPossibleMove = ComputeCells(m_playerPossibleMove[m_selectedToken]);
                                ChangeColorOfCells(m_selectedTokenPossibleMove, Colors.CadetBlue);
                            }
                        }
                        else {
                            if (m_board.Tokens[Grid.GetRow((UIElement)element), Grid.GetColumn((UIElement)element)] == m_selectedToken) {
                                ChangeColorOfCells(m_selectedTokenPossibleMove, Colors.CadetBlue);
                            }
                        }
                        
                    }
                }
            }
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (m_playerTurn) {
                // Si on a pas deja commence une sequence de mouvement
                if (m_sequenceEngaged.Count == 0)
                {
                    if (m_selectedTokenPossibleMove == null) return;
                    // Reset selected token
                    ChangeColorOfCells(m_selectedTokenPossibleMove, Colors.Khaki);
                    m_selectedToken = null;
                }
                else
                {
                    ChangeColorOfCells(m_selectedTokenPossibleMove, Colors.Khaki);
                }
            }
        }

        private void MainWindowClosing(object sender, CancelEventArgs e)
        {
            m_stopGame = true;
        }

        private List<TokenMove> ComputeCells(List<TokenMoveSequence> moveSequences) {
            List<TokenMove> moves = new List<TokenMove>();
            foreach (TokenMoveSequence sequence in moveSequences)
            {
               moves.Add(sequence.PeekMove());
            }
            return moves;
        }

        /// <summary>
        /// Change la couleur des cellules en fonction de leur accessibilite par rapport au pion selectionner
        /// </summary>
        /// <param name="moves">Les mouvements effectuable par un joueur</param>
        /// <param name="color">La nouvelle couleur des cellule</param>
        private void ChangeColorOfCells(List<TokenMove> moves, Color color)
        {
            foreach(TokenMove move in moves)
            {
                var element = GridBoard.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == move.Position.Y && Grid.GetColumn(e) == move.Position.X);
                if(element is Rectangle)
                {
                    Rectangle rectangle = element as Rectangle;
                    rectangle.Fill = new SolidColorBrush(color);
                }
            }
        }

        private void Replay(object sender, RoutedEventArgs e){
            m_board = new Board();
            DisplayBoard();
            Thread gameThread = new Thread(new ThreadStart(GameLoop));
            gameThread.Start();
        }
    }
}
