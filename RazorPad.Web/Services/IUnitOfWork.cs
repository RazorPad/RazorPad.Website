using System;

namespace RazorPad.Web.Services
{
    public interface IUnitOfWork : IDisposable
    {
        void SaveChanges();
    }
}