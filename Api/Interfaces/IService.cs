using BlazorApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Api.Interfaces
{
    public interface IService<T>
    {
        /*
        T Get(string id);
        void Add(T model);
        void Update(T model);
        void Delete(string id);
        IEnumerable<T> GetAll();*/

        Task<T> GetAsync(string id, string partitionKey);
        Task UpdateAsync(T model);
        Task DeleteAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<HttpStatusCode> AddAsync(T model);
    }
}
