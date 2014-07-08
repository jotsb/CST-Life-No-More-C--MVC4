using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimsTemplate.Models;
using System.Data.Entity;

namespace SimsTemplate.Models
{
    public enum MinigameType { BirdHunt, TowerDefense, WordSearch }
    public class LeaderboardModel : DbContext
    {
        private TechproContext db = new TechproContext();

        private IQueryable<Character> GetTop10ByMoney()
        {
            var characters = (
                        from c in db.Characters
                        orderby c.money descending
                        select c
                        ).Take(10);
            return characters;
        }

        private IQueryable<Character> GetTop10ByName()
        {
            var characters = (
                        from c in db.Characters
                        orderby c.money descending
                        select c
                        ).Take(10);
            return characters;
        }

        private IQueryable<Character> GetTop10ByScore()
        {
            var characters = (
                        from c in db.Characters
                        orderby c.money descending
                        select c
                        ).Take(10);
            return characters;
        }

        private IQueryable<Character> GetTop10ByGrades()
        {
            var characters = (
                        from c in db.Characters
                        orderby c.money descending
                        select c
                        ).Take(10);
            return characters;
        }

        private IEnumerable<Character> GetTop10ByMiniGame(string mg)
        {
            // get the collection of character to order according to the enum parameter
            List<Character> cl = new List<Character>();
            IEnumerable<Character> query;

            // get all the characters with a score in the minigame selected
            foreach (Character m in db.Characters)
            {
                foreach (MinigameInstance mi in m.MinigameInstances)
                {
                    if (mi.Minigame.title.Equals(mg) && mi.score != null) 
                    {
                        cl.Add(m);
                    }
                }
            }

            //var characters = (
            //                    from    c in db.Characters
            //                    where   c.MinigameInstances.Where(mi => mi.Minigame.title.Equals(mg))
            //                    orderby c.MinigameInstances.Where(mi => mi.score)
            //                    select  c
            //                 ).Take(10);


            return query = (cl.OrderBy(ch => (ch.MinigameInstances).OrderBy(mi => mi.score))).Take(10);
        }
    }
}
