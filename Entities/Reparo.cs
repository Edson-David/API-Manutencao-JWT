using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manutencao.Entities
{
    public class Reparo
    {
        public int Id { get; set; }
        public string Maquina { get; set; }
        public string Local { get; set; }
        public string Problema { get; set; }
        public EnumStatus Status { get; set; }
    }
}