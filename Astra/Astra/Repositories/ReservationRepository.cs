using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using Astra.DatabaseContext;
using Astra.Models;

namespace Astra.Repositories
{
  public class ReservationRepository : IReservationRepository
  {
    public const int MAX_RESERVATIONS = 3;

    private AstraContext _context = null;
    public ReservationRepository(AstraContext context)
    {
      _context = context;
    }

    public void ReserveBook(string mid, int resourceId)
    {
      var active = GetActiveReservationsForResource(resourceId);

      if (active.Where(x => x.UserMID.ToUpper() == mid.ToUpper()).Any())
      {
        throw new Exception("User has already checked out this resource.");
      }

      if (active.Count() < MAX_RESERVATIONS)
      {
        _context.Reservations.Add(new Reservation()
        {
          UserMID = mid,
          ResourceID = resourceId,
          ReservationStatus = ReservationStatus.Reserved
        });

        _context.SaveChanges();
      }
      else
      {
        throw new Exception("Maximum number of reservations reached.");
      }
    }

    public void AcceptReservation(int reservationId)
    {
      Reservation targetReservation = _context.Reservations.Where(x => x.ReservationID == reservationId).First();

      targetReservation.ReservationStatus = ReservationStatus.Accepted;

      _context.SaveChanges();
    }

    public int GetNumberActiveReservationsForResource(int resourceId)
    {
      return this.GetActiveReservationsForResource(resourceId).Count();
    }

    public void Dispose()
    {
      // Do nothing
    }

    public IEnumerable<Reservation> GetActiveReservationsForResource(int resourceId)
    {
      return _context.Reservations.Where(x => x.ResourceID == resourceId && x.ReservationStatus == ReservationStatus.Reserved).ToList();
    }

    public bool DoesUserHaveResourceReserved(string mid, int resourceId)
    {
      return _context.Reservations.Where(x => x.ResourceID == resourceId
        && x.ReservationStatus == ReservationStatus.Reserved
        && x.UserMID.ToUpper() == mid.ToUpper()).Any();
    }

    public void CancelReservation(string mid, int resourceId)
    {
      var search = _context.Reservations.Where(x => x.ResourceID == resourceId && x.UserMID == mid && x.ReservationStatus == ReservationStatus.Reserved);

      if (!search.Any())
      {
        throw new Exception("Could not locate target resource.");
      }

      var searchResult = search.First();

      searchResult.ReservationStatus = ReservationStatus.Cancelled;

      _context.SaveChanges();
    }

    public Reservation AcceptOldestReservation(int resourceId)
    {
      var search = _context.Reservations.Where(x => x.ReservationStatus == ReservationStatus.Reserved && x.ResourceID == resourceId).OrderBy(x => x.DateReserved);

      if (search.Any())
      {
        var result = search.First();
        _context.Reservations.Remove(result);
        _context.SaveChanges();
        return result;
      }

      return null;
    }
    public IEnumerable<Reservation> GetAllReservationsForUser(string userMID)
    {
      if (userMID != null)
      {
        var reservations = _context.Reservations.Where(x => x.UserMID.ToUpper() == userMID.ToUpper() && x.ReservationStatus == ReservationStatus.Reserved).OrderByDescending(x => x.DateReserved).Include("Resource").ToList();
        return reservations;
      }
      return null;
    }
  }

  public interface IReservationRepository : IDisposable
  {
    void ReserveBook(string mid, int resourceId);
    void CancelReservation(string mid, int resourceId);
    void AcceptReservation(int reservationId);
    Reservation AcceptOldestReservation(int resourceId);
    int GetNumberActiveReservationsForResource(int reservationId);
    IEnumerable<Reservation> GetActiveReservationsForResource(int reservationId);
    IEnumerable<Reservation> GetAllReservationsForUser(string userMID);
    bool DoesUserHaveResourceReserved(string mid, int resourceId);
  }
}