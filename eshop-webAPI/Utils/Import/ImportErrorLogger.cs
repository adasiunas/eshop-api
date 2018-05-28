using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Utils.Import
{
    public class ImportErrorLogger
    {
        private ILogger<ControllerBase> _logger;
        public List<ImportError> errors { get; private set; }

        public ImportErrorLogger(ILogger<ControllerBase> logger)
        {
            _logger = logger;
            errors = new List<ImportError>();
        }

        public void LogError(string error)
        {
            LogError(null, error);
        }

        public void LogError(int? row, string error)
        {
            _logger.LogInformation(error);
            errors.Add(new ImportError
            {
                Row = row,
                Error = error
            });
        }
    }

    public class ImportError
    {
        public int? Row { get; set; }
        public string Error { get; set; }
    }
}
