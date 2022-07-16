using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQConvertDbTableToExcel.Shared
{
    public class CreateExcelMessage
    {
        public string UserFileId { get; set; }
    }
}
