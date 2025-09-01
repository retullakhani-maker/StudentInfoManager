using Core.Interfaces;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class StateService : IStateService
    {
        private readonly IStateRepository _stateRepository;

        public StateService(IStateRepository stateRepository)
        {
            _stateRepository = stateRepository;
        }

        public Task<IEnumerable<States>> GetAllStatesAsync()
        {
            return _stateRepository.GetAllAsync();
        }

        public Task<States> GetStateByIdAsync(int id)
        {
            return _stateRepository.GetByIdAsync(id);
        }
    }
}
