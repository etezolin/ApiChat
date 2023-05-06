using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiRestFull.Model
{
    public class ProdutoViewModel
    {
        [Required(ErrorMessage = "ProdutoID é um campo obrigatório.")]
        public int ProdutoID { get; set; }
        [Required(ErrorMessage = "ProdutoNome é um campo obrigatório.")]
        public string ProdutoNome { get; set; }
        [Required(ErrorMessage = "Categoria é um campo obrigatório.")]
        public string Categoria { get; set; }
        [Required(ErrorMessage = "Preco é um campo obrigatório.")]
        public double Preco { get; set; }
    }
}
