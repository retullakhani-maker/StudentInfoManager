using Core.Interfaces;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;

        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public Task<IEnumerable<Cities>> GetCitiesByStateIdAsync(int stateId)
        {
            return _cityRepository.GetByStateIdAsync(stateId);
        }
    }
}
