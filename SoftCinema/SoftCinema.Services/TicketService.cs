﻿using System;
using SoftCinema.Data;
using SoftCinema.Models;

namespace SoftCinema.Services
{
    public static class TicketService
    {
        //TODO: Additional checks and applying discount to ticket price
        public static void AddTicket(Screening screening, TicketType type, Seat seat)
        {
            using (SoftCinemaContext context = new SoftCinemaContext())
            {
                if (!AuthenticationManager.IsAuthenticated())
                {
                    throw new InvalidOperationException("You must log in first!");
                }

                User holder = AuthenticationManager.GetCurrentUser();

                Ticket ticket = new Ticket(holder.Id, screening.Id, seat.Id, type);
               

                context.Tickets.Add(ticket);
                context.SaveChanges();
            }
        }
    }
}