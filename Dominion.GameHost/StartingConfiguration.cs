﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dominion.Cards.Curses;
using Dominion.Cards.Treasure;
using Dominion.Cards.Victory;
using Dominion.Rules;

namespace Dominion.GameHost
{
    public abstract class StartingConfiguration
    {
        private readonly int _numberOfPlayers;

        public StartingConfiguration(int numberOfPlayers)
        {
            if(numberOfPlayers < 1)
                throw new ArgumentException("Number of players must be greater than 0", "numberOfPlayers");

            _numberOfPlayers = numberOfPlayers;
        }

        public virtual void InitializeBank(CardBank bank)
        {
            bank.AddCardPile(Copper);
            bank.AddCardPile(Silver);
            bank.AddCardPile(Gold);
            bank.AddCardPile(Estates);
            bank.AddCardPile(Duchies);
            bank.AddCardPileWhichEndsTheGameWhenEmpty(Provinces);
            bank.AddCardPile(Curses);
        }

        public Game CreateGame(IEnumerable<string> playerNames)
        {
            if(playerNames.Count() != _numberOfPlayers)
                throw new ArgumentException(string.Format("Expected {0} player names.", playerNames));

            var bank = new CardBank();
            InitializeBank(bank);

            var players = playerNames.Select(name => new Player(name, CreateStartingDeck()));
            return new Game(players, bank, new TextGameLog());
        }

        public IEnumerable<Card> CreateStartingDeck()
        {
            return 3.NewCards<Estate>().Concat(7.NewCards<Copper>());
            //var cards = new List<Card>();
            //cards.AddRange(3.NewCards<Dominion.Cards.Actions.ThroneRoom>());
            //cards.AddRange(2.NewCards<Dominion.Cards.Actions.Witch>());
            //cards.AddRange(2.NewCards<Dominion.Cards.Actions.Market>());            
            //cards.AddRange(3.NewCards<Gold>());
            //return cards;
        }

        private int VictoryCardCount
        {
            get { return (Math.Max(0, _numberOfPlayers - 4) * 3) + 12; }
        }

        private CardPile Copper
        {
            get { return new UnlimitedSupplyCardPile(() => new Copper()); }
        }

        private CardPile Silver
        {
            get { return new UnlimitedSupplyCardPile(() => new Silver()); }
        }

        private CardPile Gold
        {
            get { return new UnlimitedSupplyCardPile(() => new Gold()); }
        }

        private CardPile Estates
        {
            get  { return new LimitedSupplyCardPile().WithNewCards<Estate>(VictoryCardCount); }
        }

        private CardPile Duchies
        {
            get { return new LimitedSupplyCardPile().WithNewCards<Duchy>(VictoryCardCount); }
        }

        private CardPile Provinces
        {
            get { return new LimitedSupplyCardPile().WithNewCards<Province>(VictoryCardCount); }
        }

        private CardPile Curses
        {
            get { return new LimitedSupplyCardPile().WithNewCards<Curse>(Math.Max(10, (_numberOfPlayers - 1) * 10)); }            
        }
    }
}