using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using MSOrleansDemo.Context;
using Orleans.Runtime;
using System;
using System.Threading;

namespace MSOrleansDemo.Grains
{
    public class AgreementEFGrain : Grain, IAgreementEFGrain
    {
        private AgreementState _state;

        public AgreementEFGrain()
        {
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            using (var scope = base.ServiceProvider.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<OrleansDemoDbContext>())
                {
                    _state = await context.GetByAgreementIdAsync(this.GetPrimaryKeyString());

                    if (_state is null)
                    {
                        _state = new AgreementState();
                        _state.AgreementId = this.GetPrimaryKeyString();
                        await context.AgreementStates.AddAsync(_state, cancellationToken);
                        await context.SaveChangesAsync(cancellationToken);
                    }
                }
            }

            await base.OnActivateAsync(cancellationToken);
        }

        private async Task SaveStateAsync()
        {
            using (var scope = base.ServiceProvider.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<OrleansDemoDbContext>())
                {
                    context.AgreementStates.Update(_state);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task<string> GeneratePdfAsync()
        {
            _state.PdfFileLocation = Guid.NewGuid().ToString() + ".pdf";
            await SaveStateAsync();
            return _state.PdfFileLocation;
        }

        /// <summary>
        /// GetState convention for Orleans Dashboard to retrive grain state dynamically
        /// </summary>
        /// <returns></returns>
        public async Task<AgreementDetails> GetState()
        {
            return new AgreementDetails()
            {
                PdfFileLocation = _state.PdfFileLocation,
                SignerId = _state.SignerId
            };
        }

        public async Task SignAsync(string signerId)
        {
             _state.SignerId = signerId;
            await SaveStateAsync();

            var brokerGrain = GrainFactory.GetGrain<IBrokerGrain>(0);

            await brokerGrain.SendMessageAsync(new AgreementSignedEvent
            {
                AgreementId = this.GetPrimaryKeyString(),
                PdfFileLocation = _state.PdfFileLocation,
                SignerId = signerId
            });
        }
    }
}