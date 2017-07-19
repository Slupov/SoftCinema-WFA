﻿using SoftCinema.Data;
using SoftCinema.Models;
using SoftCinema.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;

namespace SoftCinema.Services
{
    public class TicketService
    {
        public void AddTicketToCurrentUser(Screening screening, TicketType type, Seat seat)
        {
            if (AuthenticationManager.IsAuthenticated())
            {
                throw new InvalidOperationException(Services.Utilities.Constants.ErrorMessages.LoginFirst);
            }
            int screeningId = screening.Id;
            int seatId = seat.Id;
            int userId = AuthenticationManager.GetCurrentUser().Id;
            AddTicket(screeningId, type, seatId, userId);
        }

        public void AddTicket(int screeningId, TicketType type, int seatId, int holderId)
        {
            using (SoftCinemaContext context = new SoftCinemaContext())
            {
                decimal price = 0;
                switch (type)
                {
                    case TicketType.Children:
                        price = Constants.ChildrenTicketPrice;
                        break;

                    case TicketType.Students:
                        price = Constants.StudentsTicketPrice;
                        break;

                    case TicketType.Regular:
                        price = Constants.RegularTicketPrice;
                        break;

                    case TicketType.Seniors:
                        price = Constants.SeniorsrTicketPrice;
                        break;

                    default:
                        break;
                }
                Ticket ticket = new Ticket()
                {
                    HolderId = holderId,
                    ScreeningId = screeningId,
                    SeatId = seatId,
                    Type = type,
                    Price = price
                };
                context.Tickets.Add(ticket);
                context.SaveChanges();
            }
        }

        public List<Ticket> GetTicketsInfo(string username)
        {
            using (var db = new SoftCinemaContext())
            {
                return db.Tickets
                    .Where(t => t.Holder.Username == username)
                    .Include(t => t.Screening.Auditorium)
                    .Include(t => t.Screening.Auditorium.Cinema)
                    .Include(t => t.Screening.Auditorium.Cinema.Town)
                    .Include(t => t.Seat)
                    .Include(t => t.Screening.Movie.Image)
                    .Include(t => t.Screening.Movie)
                    .ToList();
            }
        }

        public List<Ticket> GetTickets(Screening screening)
        {
            using (var db = new SoftCinemaContext())
            {
                return db.Tickets
                    .Include("Seat")
                    .Include("Holder")
                    .Where(t => t.ScreeningId == screening.Id).ToList();
            }
        }

        public Ticket GetTicket(int holderId, int seatId, int screeningId)
        {
            using (SoftCinemaContext context = new SoftCinemaContext())
            {
                return
                    context.Tickets
                        .Include(t => t.Screening.Auditorium)
                        .Include(t => t.Screening.Auditorium.Cinema)
                        .Include(t => t.Screening.Auditorium.Cinema.Town)
                        .Include(t => t.Seat)
                        .Include(t => t.Screening.Movie.Image)
                        .Include(t => t.Screening.Movie)
                        .FirstOrDefault(
                            t => t.HolderId == holderId && t.SeatId == seatId && t.ScreeningId == screeningId);
            }
        }

        public Ticket GetTicket(int number, Screening screening)
        {
            using (SoftCinemaContext context = new SoftCinemaContext())
            {
                return context.Tickets
                    .Include("Seat")
                    .Include("Screening")
                    .FirstOrDefault(t => t.Seat.Number == number && t.ScreeningId == screening.Id);
            }
        }

        public void UpdateTicket(int ticketId, int screeningId, int seatId, TicketType type)
        {
            using (SoftCinemaContext context = new SoftCinemaContext())
            {
                Ticket ticket = context.Tickets.Find(ticketId);
                ticket.ScreeningId = screeningId;
                ticket.SeatId = seatId;
                ticket.Type = type;
                context.SaveChanges();
            }
        }

        public void RemoveTicket(int ticketId)
        {
            using (SoftCinemaContext context = new SoftCinemaContext())
            {
                Ticket ticket = context.Tickets.Find(ticketId);
                context.Tickets.Remove(ticket);
                context.SaveChanges();
            }
        }

        public string GetTicketDate(int ticketId)
        {
            using (SoftCinemaContext context = new SoftCinemaContext())
            {
                Ticket ticket = context.Tickets.Find(ticketId);
                return ticket.Screening.Start.Day + " " + ticket.Screening.Start.Date.ToString("MMM") + " " +
                       ticket.Screening.Start.DayOfWeek;
            }
        }

        public string GetTicketTime(int ticketId)
        {
            using (SoftCinemaContext context = new SoftCinemaContext())
            {
                Ticket ticket = context.Tickets.Find(ticketId);
                return ticket.Screening.Start.ToString("hh") + ":" + ticket.Screening.Start.ToString("mm") + " " +
                       ticket.Screening.Start.ToString("tt", CultureInfo.InvariantCulture);
            }
        }

        public void SellTickets(List<Ticket> tickets)
        {
            using (var db = new SoftCinemaContext())
            {
                foreach (var ticket in tickets)
                {
                    db.Tickets.Attach(ticket);
                    ticket.isPaid = true;
                }
                db.SaveChanges();
            }
        }

        public void SellTicket(Ticket ticket)
        {
            using (var db = new SoftCinemaContext())
            {
                db.Tickets.Attach(ticket);
                ticket.isPaid = true;

                db.SaveChanges();
            }
        }

        public void DeleteTicket(Ticket ticket)
        {
            using (var db = new SoftCinemaContext())
            {
                db.Tickets.Attach(ticket);
                ticket.Screening = null;
                ticket.Holder = null;
                ticket.Seat = null;
                db.Tickets.Remove(ticket);

                db.SaveChanges();
            }
        }
    }
}