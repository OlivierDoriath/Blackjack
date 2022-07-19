using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Blackjack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class Hand
    {
        public int Score;
        public int Count;
        private List<Card> cards = new();

        private void CalculateScore()
        {
            int tempScore = 0;
            int numOfA = 0;
            foreach (Card c in cards)
            {
                if ("TJQK".Contains(c.value))
                {
                    tempScore += 10;
                }
                else if (c.value == 'A')
                {
                    numOfA++;
                    tempScore++;
                }
                else
                {
                    tempScore += int.Parse(c.value.ToString());
                }
            }
            while (tempScore <= 11 && numOfA > 0)
            {
                tempScore += 10;
                numOfA--;
            }
            Score = tempScore;
        }

        public void AddCard(Card c)
        {
            cards.Add(c);
            Count++;
            CalculateScore();
        }

        public List<Card> GetCards()
        {
            return cards;
        }
    }
    public class Deck
    {
        public List<Card> cards;

        public Deck()
        {
            cards = new List<Card>();
            List<Card> tempCards = new();
            string values = "23456789TJQKA";
            string colors = "HCDS";
            foreach (char v in values)
            {
                foreach (char c in colors)
                {
                    tempCards.Add(new Card(v, c));
                }
            }
            Random r = new(DateTime.Now.Millisecond);
            while (tempCards.Count > 0)
            {
                int randNum = r.Next(tempCards.Count - 1);
                cards.Add(tempCards[randNum]);
                tempCards.RemoveAt(randNum);
            }
        }
        public Card NextCard()
        {
            Card tempCard = cards.First();
            cards.Remove(tempCard);
            return tempCard;
        }
    }
    public class Card
    {
        public char value;
        public char color;
        public Card(char v, char c)
        {
            value = v;
            color = c;
        }

    }
    public partial class MainWindow : Window
    {
        private int funds = 100;
        private int currentBet;
        private Hand playerHand = new();
        private Hand dealerHand = new();
        private Deck deck = new();
        public MainWindow()
        {
            InitializeComponent();
            betPanel.Visibility = Visibility.Hidden;
            fundsLabel.Visibility = Visibility.Hidden;
            betLabel.Visibility = Visibility.Hidden;
            playPanel.Visibility = Visibility.Hidden;
            playerScore.Visibility = Visibility.Hidden;
            dealerScore.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PlayGame();
        }

        private void betOneButton_Click(object sender, RoutedEventArgs e)
        {
            Bet(1);
        }

        private void betFiveButton_Click(object sender, RoutedEventArgs e)
        {
            Bet(5);
        }

        private void betTenButton_Click(object sender, RoutedEventArgs e)
        {
            Bet(10);
        }

        private void betTwentyfiveButton_Click(object sender, RoutedEventArgs e)
        {
            Bet(25);
        }

        private void betFiftyButton_Click(object sender, RoutedEventArgs e)
        {
            Bet(50);
        }

        private void allInButton_Click(object sender, RoutedEventArgs e)
        {
            Bet(funds);
        }

        private void standButton_Click(object sender, RoutedEventArgs e)
        {
            Result();
        }

        private void dealButton_Click(object sender, RoutedEventArgs e)
        {
            DealCard(deck.NextCard(), "Player", false);
        }

        private void doubleButton_Click(object sender, RoutedEventArgs e)
        {
            doubleButton.IsEnabled = false;
            currentBet *= 2;
            betLabel.Content = "Bet: $" + currentBet;
            DealCard(deck.NextCard(), "Player", false);
        }

        private void PlayGame()
        {
            if (funds == 0)
            {
                MessageBox.Show("You're out of money! Better luck next time!");
                Environment.Exit(0);
            }
            playerHand = new Hand();
            playerCards.Children.Clear();
            dealerHand = new Hand();
            dealerCards.Children.Clear();
            currentBet = 0;
            playButton.Visibility = Visibility.Hidden;
            title.Visibility = Visibility.Hidden;
            betPanel.Visibility = Visibility.Visible;
            fundsLabel.Visibility = Visibility.Visible;
            playPanel.Visibility = Visibility.Hidden;
            betLabel.Visibility = Visibility.Hidden;
            playerScore.Visibility = Visibility.Hidden;
            dealerScore.Visibility = Visibility.Hidden;
            betLabel.Content = "Bet: $" + currentBet;
            fundsLabel.Content = "Funds: $" + funds;
        }

        private void DealCard(Card c, string target, bool final)
        {
            if (deck.cards.Count == 0)
            {
                deck = new Deck();
            }

            int cardCol = "23456789TJQKA".IndexOf(c.value);
            int cardRow = "HCDS".IndexOf(c.color);
            Image cardPicture = new();
            BitmapImage cardGraphics = new(new Uri(@"../../../8BitDeckAssets.png", UriKind.Relative));

            if (target == "Dealer" && dealerHand.Count >= 1 && final == false)
            {
                cardPicture.Source = new CroppedBitmap(cardGraphics, new Int32Rect(0, 0, 35, 47));
            }
            else
            {
                cardPicture.Source = new CroppedBitmap(cardGraphics, new Int32Rect((1 + cardCol) * 35, cardRow * 47, 35, 47));
            }

            if (target == "Player")
            {
                playerCards.Children.Add(cardPicture);
                playerHand.AddCard(c);
                playerScore.Content = "Score: " + playerHand.Score;
            }
            else
            {
                dealerCards.Children.Add(cardPicture);
                if (!final)
                {
                    dealerHand.AddCard(c);
                }

                dealerScore.Content = "Score: " + dealerHand.Score;
            }

            if (playerHand.Score >= 21 && target == "Player")
            {
                Result();
            }
        }
        private void Bet(int bet)
        {
            if (bet <= funds)
            {
                playerHand = new Hand();
                dealerHand = new Hand();

                betPanel.Visibility = Visibility.Hidden;
                playPanel.Visibility = Visibility.Visible;
                playerScore.Visibility = Visibility.Visible;
                fundsLabel.Visibility = Visibility.Hidden;
                betLabel.Visibility = Visibility.Visible;
                currentBet = bet;
                betLabel.Content = "Bet: $" + currentBet;

                InitialDeal();
            }
            else
            {
                MessageBox.Show("You cant bet what you don't have!", "Warning!", MessageBoxButton.OK);
            }
        }

        private void InitialDeal()
        {
            doubleButton.IsEnabled = funds >= currentBet * 2;

            DealCard(deck.NextCard(), "Player", false);
            DealCard(deck.NextCard(), "Dealer", false);
            DealCard(deck.NextCard(), "Player", false);
            DealCard(deck.NextCard(), "Dealer", false);
        }

        private void Result()
        {
            dealerScore.Visibility = Visibility.Visible;
            while (dealerHand.Score < 17)
            {
                DealCard(deck.NextCard(), "Dealer", false);
            }
            if (playerHand.Score > 21)
            {

                dealerCards.Children.Clear();
                foreach (Card c in dealerHand.GetCards())
                {
                    DealCard(c, "Dealer", true);
                }
                funds -= currentBet;
                MessageBox.Show("You busted! You lost a total of $" + currentBet);
                PlayGame();
                return;
            }
            else
            {

                dealerCards.Children.Clear();
                foreach (Card c in dealerHand.GetCards())
                {
                    DealCard(c, "Dealer", true);
                }

                if (dealerHand.Score > 21)
                {
                    funds += currentBet;
                    MessageBox.Show("The dealer busted! You won a total of $" + currentBet);
                }
                else if (dealerHand.Score > playerHand.Score)
                {
                    funds -= currentBet;
                    MessageBox.Show("The dealer won! You lost a total of $" + currentBet);
                }
                else if (dealerHand.Score < playerHand.Score)
                {
                    funds += currentBet;
                    MessageBox.Show("You beat the dealer! You won a total of $" + currentBet);
                }
                else if (dealerHand.Score == playerHand.Score)
                {
                    MessageBox.Show("It's a push!");
                }
                PlayGame();
                return;

            }

        }
    }
}
