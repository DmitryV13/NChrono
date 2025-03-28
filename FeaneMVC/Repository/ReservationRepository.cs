using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalProject.DbModel;
using FinalProject.Models;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Models.Enums;
using WebApplication1.Models.Response;

namespace WebApplication1.Repository
{
    public class ReservationRepository : IReservation
    {
        private readonly ApplicationDbContext _context;
        private readonly INotification _notification;

        public ReservationRepository(ApplicationDbContext context, INotification notification)
        {
            _context = context;
            _notification = notification;
        }

        // Create a new reservation
        public ReservationResponse CreateReservation(Reservation reservation, Guid userId)
        {
            if (reservation == null)
            {
                return new ReservationResponse
                {
                    Message = "Reservation data is null",
                    Status = false
                };
            }

            try
            {
                // Validate reservation date and time
                var conflictingReservation =  _context.Reservations
                    .FirstOrDefault(r => r.ReservationDate.Date == reservation.ReservationDate.Date &&
                                               r.ReservationDate.TimeOfDay == reservation.ReservationDate.TimeOfDay);

                if (conflictingReservation != null)
                {
                    return new ReservationResponse
                    {
                        Message = "Conflict with an existing reservation at this time.",
                        Status = false
                    };
                }

                // Set the reservation status (e.g., Confirmed)
                reservation.Status = ReservationStatus.Confirmed;
                reservation.UserId = userId;

                var reservationHistory = new ReservationHistory
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ReservationDate = reservation.ReservationDate,
                    Status = reservation.Status,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Add reservation to the context and save changes
                 _context.ReservationsHistory.Add(reservationHistory);
                 _context.Reservations.Add(reservation);
                 _context.SaveChanges();

                // Optionally: Send a confirmation to the user
                var user =  _context.Users.Find(userId);
                if (user != null)
                {
                    var message = $"Dear {reservation.CustomerName}, your reservation for {reservation.NumberOfPeople} people on {reservation.ReservationDate} has been created successfully.";
                    _notification.SendNotification(message, reservation.UserEmail);
                }

                return new ReservationResponse
                {
                    Status = true,
                    Message = "Reservation created successfully",
                    Reservation = reservation
                };
            }
            catch (Exception ex)
            {
                // Log the exception (use a logging framework in a real application)
                // For example: _logger.LogError(ex, "Error creating reservation");
                return new ReservationResponse
                {
                    Message = "Error creating reservation",
                    Status = false
                };
            }
        }

        // Cancel a reservation
        public ReservationResponse CancelReservation(Guid reservationId)
        {
            if (reservationId == Guid.Empty)
            {
                return new ReservationResponse
                {
                    Message = "Invalid reservation ID",
                    Status = false
                };
            }

            try
            {
                var reservation =  _context.Reservations.Find(reservationId);
                if (reservation == null)
                {
                    return new ReservationResponse
                    {
                        Message = "Reservation not found",
                        Status = false
                    };
                }

                _context.Reservations.Remove(reservation);
                 _context.SaveChanges();

                return new ReservationResponse
                {
                    Status = true,
                    Message = "Reservation cancelled successfully"
                };
            }
            catch (Exception ex)
            {
                // Log the exception
                // For example: _logger.LogError(ex, "Error cancelling reservation");
                return new ReservationResponse
                {
                    Message = "Error cancelling reservation",
                    Status = false
                };
            }
        }

        // Retrieve a reservation by ID
        public ReservationResponse GetReservationById(Guid reservationId)
        {
            if (reservationId == Guid.Empty)
            {
                return new ReservationResponse
                {
                    Message = "Invalid reservation ID",
                    Status = false
                };
            }

            try
            {
                var reservation =  _context.Reservations.Find(reservationId);
                if (reservation == null)
                {
                    return new ReservationResponse
                    {
                        Message = "Reservation not found",
                        Status = false
                    };
                }

                return new ReservationResponse
                {
                    Status = true,
                    Message = "Reservation retrieved successfully",
                    Reservation = reservation
                };
            }
            catch (Exception ex)
            {
                // Log the exception
                // For example: _logger.LogError(ex, "Error fetching reservation by ID");
                return new ReservationResponse
                {
                    Message = "Error fetching reservation",
                    Status = false
                };
            }
        }

        // Retrieve all reservations
        public IEnumerable<Reservation> GetAllReservations()
        {
            try
            {
                return  _context.Reservations.ToList();
            }
            catch (Exception ex)
            {
                // Log the exception
                // For example: _logger.LogError(ex, "Error fetching all reservations");
                return new List<Reservation>();
            }
        }

        // Retrieve reservations by user ID
        public IEnumerable<Reservation> GetReservationsByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return new List<Reservation>();
            }

            try
            {
                return  _context.Reservations.Where(r => r.UserId == userId).ToList();
            }
            catch (Exception ex)
            {
                // Log the exception
                // For example: _logger.LogError(ex, "Error fetching reservations by user ID");
                return new List<Reservation>();
            }
        }

        // Update a reservation
        public ReservationResponse UpdateReservation(Guid reservationId, Reservation reservation)
        {
            if (reservationId == Guid.Empty || reservation == null)
            {
                return new ReservationResponse
                {
                    Message = "Invalid input",
                    Status = false
                };
            }

            try
            {
                var existingReservation =  _context.Reservations.Find(reservationId);
                if (existingReservation == null)
                {
                    return new ReservationResponse
                    {
                        Message = "Reservation not found",
                        Status = false
                    };
                }

                // Update fields
                existingReservation.ReservationDate = reservation.ReservationDate;
                existingReservation.UserId = reservation.UserId;
                existingReservation.UpdatedAt = DateTime.UtcNow;

                 _context.SaveChanges();

                return new ReservationResponse
                {
                    Status = true,
                    Message = "Reservation updated successfully",
                    Reservation = existingReservation
                };
            }
            catch (Exception ex)
            {
                // Log the exception
                // For example: _logger.LogError(ex, "Error updating reservation");
                return new ReservationResponse
                {
                    Message = "Error updating reservation",
                    Status = false
                };
            }
        }


    }
}