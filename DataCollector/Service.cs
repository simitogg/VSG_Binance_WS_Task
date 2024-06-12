using Commons;
using System.ServiceProcess;

namespace DataCollector
{
    partial class Service : ServiceBase
    {
        private readonly SharedContext _context;
        public Service(SharedContext context)
        {
            InitializeComponent();
            _context = context;
        }

        protected override void OnStart(string[] args)
        {
            var binanceService = new WSService(_context);
            binanceService.StartAsync().GetAwaiter().GetResult();
        }

        protected override void OnStop()
        {
            _context.Dispose();
        }
    }
}
