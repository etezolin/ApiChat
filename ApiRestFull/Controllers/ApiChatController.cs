using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ApiChat.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ApiChat.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApiChatController : ControllerBase
    {
        [HttpPost("Publisher/{msg}")]
        [AllowAnonymous]
        public async Task<IActionResult> MsgPublisher(string msg)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
            };

            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "Mensagem_",
                                durable: false,
                                exclusive: false,
                                autoDelete: true,
                                arguments: null
                                );
            string message = msg;

            var body = Encoding.UTF8.GetBytes(message);

            await Task.Run(() =>
            {
                channel.BasicPublish(exchange: "",
                                routingKey: "Mensagem_",
                                basicProperties: null,
                                body: body
                                );
            });
            return Ok(new { Message = "Fila publicada com sucesso!" });
        }

        [HttpGet("Consumer/Msg")]
        [AllowAnonymous]
        public async Task<IActionResult> MsgConsumer()
        {
            var lista = new List<Questoes>();
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "Mensagem_",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: true,
                                        arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var q = new Questoes
                    {
                        Questao = message
                    };
                    lista.Add(q);
                };

                channel.BasicConsume(queue: "Mensagem_",
                                        autoAck: true,
                                        consumer: consumer);

                var expectedCount = 3 /* Número esperado de mensagens */;
                while (lista.Count < expectedCount)
                {
                    await Task.Delay(100); // Espera 100ms antes de verificar novamente
                }

                return Ok(lista);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostPublisher()
        {
            string[] array = new string[3];
            array[0] = "Qual é o seu nome?";
            array[1] = "Qual é a sua idade?";
            array[2] = "Qual é a sua profissão?";

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
            };

            for (int x = 0; x < 3; x++)
            {
                using var connection = factory.CreateConnection();

                using var channel = connection.CreateModel();
                channel.QueueDeclare(queue: "Mensagem",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: true,
                                    arguments: null
                                    );
                string message = array[x];

                var body = Encoding.UTF8.GetBytes(message);

                await Task.Run(() =>
                {
                    channel.BasicPublish(exchange: "",
                                    routingKey: "Mensagem",
                                    basicProperties: null,
                                    body: body
                                    );
                });
            }

            return Ok(new { Message = "Fila publicada com sucesso!" });

        }

        [HttpGet("Consumer")]
        [AllowAnonymous]
        public async Task<IActionResult> GetConsumer()
        {
            var lista = new List<Questoes>();
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "Mensagem",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: true,
                                        arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var q = new Questoes
                    {
                        Questao = message
                    };
                    lista.Add(q);
                };

                channel.BasicConsume(queue: "Mensagem",
                                        autoAck: true,
                                        consumer: consumer);

                var expectedCount = 3 /* Número esperado de mensagens */;
                while (lista.Count < expectedCount)
                {
                    await Task.Delay(100); // Espera 100ms antes de verificar novamente
                }

                return Ok(lista);
            }
            //var lista = new List<Questoes>();
            //var factory = new ConnectionFactory()
            //{
            //    HostName = "localhost"
            //};

            //var tcs = new TaskCompletionSource<List<Questoes>>();

            //using (var connection = factory.CreateConnection())
            //using (var channel = connection.CreateModel())
            //{
            //    channel.QueueDeclare(queue: "Mensagem",
            //                            durable: false,
            //                            exclusive: false,
            //                            autoDelete: true,
            //                            arguments: null);

            //    var consumer = new EventingBasicConsumer(channel);
            //    consumer.Received += (model, ea) =>
            //    {
            //        var body = ea.Body.ToArray();
            //        var message = Encoding.UTF8.GetString(body);
            //        var q = new Questoes
            //        {
            //            Questao = message
            //        };
            //        lista.Add(q);
            //        if (lista.Count == Convert.ToInt32(ea.DeliveryTag))
            //        {
            //            tcs.SetResult(lista);
            //        }
            //        //if (lista.Count == ea.DeliveryTag)
            //        //{
            //        //    tcs.SetResult(lista);
            //        //}
            //    };

            //    channel.BasicConsume(queue: "Mensagem",
            //                            autoAck: true,
            //                            consumer: consumer);

            //    await tcs.Task;
            //}

            //return Ok(lista);

            //#############################################################################################################

            //var lista = new List<Questoes>();
            //var factory = new ConnectionFactory()
            //{
            //    HostName = "localhost"
            //};

            //try
            //{
            //    using (var connection = factory.CreateConnection())
            //    using (var channel = connection.CreateModel())
            //    {
            //        channel.QueueDeclare(queue: "Mensagem",
            //                                durable: false,
            //                                exclusive: false,
            //                                autoDelete: true,
            //                                arguments: null);

            //        var consumer = new EventingBasicConsumer(channel);
            //        consumer.Received += async (model, ea) =>
            //        {
            //            var body = ea.Body.ToArray();
            //            var message = Encoding.UTF8.GetString(body);
            //            var q = new Questoes
            //            {
            //                Questao = message
            //            };
            //            lista.Add(q);
            //            await Task.Yield();
            //        };

            //        channel.BasicConsume(queue: "Mensagem",
            //                                autoAck: true,
            //                                consumer: consumer);
            //    }

            //    return Ok(lista);
            //}
            //    catch (Exception ex)
            //    {
            //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            //    }
        }

        //var factory = new ConnectionFactory()
        //{
        //    HostName = "localhost",
        //};

        //using (var connection = factory.CreateConnection())

        //using (var channel = connection.CreateModel())
        //{
        //    channel.QueueDeclare(queue: "Mensagem",
        //                        durable: false,
        //                        exclusive: false,
        //                        autoDelete: true,
        //                        arguments: null
        //                        );

        //    var consumer = new EventingBasicConsumer(channel);

        //    consumer.Received += async (model, ea) =>
        //    {
        //        var body = ea.Body.ToArray();
        //        var message = Encoding.UTF8.GetString(body);
        //        var q = new Questoes
        //        {
        //            Questao = message
        //        };
        //        lista.Add(q);
        //        await Task.Delay(1000);
        //        //Console.WriteLine($"[x] Recebida: {message}");

        //    };

        //    await Task.Run(() =>
        //    {
        //        Task.Delay(5000);
        //        channel.BasicConsume(queue: "Mensagem",
        //                        autoAck: true,
        //                        consumer: consumer);
        //    });



        //    //Console.ReadLine();
        //}

        //    return Ok(lista);
        //}

        //[HttpGet("GetAll")]
        ////[Authorize]
        //[AllowAnonymous]
        //public async Task<IActionResult> GetAll()
        //{
        //    var questao = new List<Questoes>();

        //    var q1 = new Questoes
        //    {
        //        Questao = "Qual é o seu nome?"
        //    };

        //    var q2 = new Questoes
        //    {
        //        Questao = "Qual é a sua idade?"
        //    };

        //    var q3 = new Questoes
        //    {
        //        Questao = "Qual é a sua profissão?"
        //    };

        //    questao.Add(q1);
        //    questao.Add(q2);
        //    questao.Add(q3);

        //    return Ok(questao);
        //}

        //[HttpGet("getById/{id}")]
        //[Authorize]
        ////[AllowAnonymous]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    var produtos = new List<ProdutoViewModel>();

        //    var produto = new ProdutoViewModel
        //    {
        //        ProdutoID = 1,
        //        ProdutoNome = "Monitor Acer 27 polegadas",
        //        Categoria = "Informática",
        //        Preco = 1400.00
        //    };

        //    produtos.Add(produto);

        //    var produto2 = new ProdutoViewModel
        //    {
        //        ProdutoID = 2,
        //        ProdutoNome = "Notebook Lenovo 14 polegadas",
        //        Categoria = "Informática",
        //        Preco = 2400.00
        //    };

        //    produtos.Add(produto2);

        //    var produto3 = new ProdutoViewModel
        //    {
        //        ProdutoID = 3,
        //        ProdutoNome = "Celular motorola g7",
        //        Categoria = "Telefonia",
        //        Preco = 700.00
        //    };

        //    produtos.Add(produto3);

        //    foreach (var x in produtos)
        //    {
        //        if (x.ProdutoID == id)
        //            return Ok(x);
        //    }

        //    return BadRequest(new { ErrorMessage = "O ID informado não existe no banco de dados." });
        //}

        //[HttpPost]
        //[Authorize]
        ////[AllowAnonymous]
        //public async Task<IActionResult> Post(ProdutoViewModel p)
        //{
        //    if (p.ProdutoID == 0)
        //        return BadRequest(new { Message = "ID zero não pode ser inserido." });

        //    var produtos = new List<ProdutoViewModel>();

        //    var produto = new ProdutoViewModel
        //    {
        //        ProdutoID = 1,
        //        ProdutoNome = "Monitor Acer 27 polegadas",
        //        Categoria = "Informática",
        //        Preco = 1400.00
        //    };

        //    produtos.Add(produto);

        //    var produto2 = new ProdutoViewModel
        //    {
        //        ProdutoID = 2,
        //        ProdutoNome = "Notebook Lenovo 14 polegadas",
        //        Categoria = "Informática",
        //        Preco = 2400.00
        //    };

        //    produtos.Add(produto2);

        //    var produto3 = new ProdutoViewModel
        //    {
        //        ProdutoID = 3,
        //        ProdutoNome = "Celular motorola g7",
        //        Categoria = "Telefonia",
        //        Preco = 700.00
        //    };

        //    produtos.Add(produto3);

        //    foreach (var x in produtos)
        //    {
        //        if (p.ProdutoID == x.ProdutoID)
        //            return BadRequest(new { ErrorMessage = "Este ID já existe no banco de dados." });
        //    }

        //    produtos.Add(p);

        //    return Ok(produtos);
        //}

        //[HttpPut("putById/{id}")]
        //[Authorize]
        ////[AllowAnonymous]
        //public async Task<IActionResult> Put(ProdutoViewModel prod, int id)
        //{
        //    var produtos = new List<ProdutoViewModel>();

        //    var produto = new ProdutoViewModel
        //    {
        //        ProdutoID = 1,
        //        ProdutoNome = "Monitor Acer 27 polegadas",
        //        Categoria = "Informática",
        //        Preco = 1400.00
        //    };

        //    produtos.Add(produto);

        //    var produto2 = new ProdutoViewModel
        //    {
        //        ProdutoID = 2,
        //        ProdutoNome = "Notebook Lenovo 14 polegadas",
        //        Categoria = "Informática",
        //        Preco = 2400.00
        //    };

        //    produtos.Add(produto2);

        //    var produto3 = new ProdutoViewModel
        //    {
        //        ProdutoID = 3,
        //        ProdutoNome = "Celular motorola g7",
        //        Categoria = "Telefonia",
        //        Preco = 700.00
        //    };

        //    produtos.Add(produto3);

        //    foreach (var x in produtos)
        //    {
        //        if (x.ProdutoID == id)
        //        {
        //            x.ProdutoID = prod.ProdutoID;
        //            x.ProdutoNome = prod.ProdutoNome;
        //            x.Preco = prod.Preco;
        //            x.Categoria = prod.Categoria;
        //            return Ok(produtos);
        //        }

        //    }

        //    return BadRequest(new { ErrorMessage = "O ID não existe no banco." });
        //}



        //[HttpDelete("deleteById/{id}")]
        //[Authorize]
        ////[AllowAnonymous]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var produtos = new List<ProdutoViewModel>();

        //    var produto = new ProdutoViewModel
        //    {
        //        ProdutoID = 1,
        //        ProdutoNome = "Monitor Acer 27 polegadas",
        //        Categoria = "Informática",
        //        Preco = 1400.00
        //    };

        //    produtos.Add(produto);

        //    var produto2 = new ProdutoViewModel
        //    {
        //        ProdutoID = 2,
        //        ProdutoNome = "Notebook Lenovo 14 polegadas",
        //        Categoria = "Informática",
        //        Preco = 2400.00
        //    };

        //    produtos.Add(produto2);

        //    var produto3 = new ProdutoViewModel
        //    {
        //        ProdutoID = 3,
        //        ProdutoNome = "Celular motorola g7",
        //        Categoria = "Telefonia",
        //        Preco = 700.00
        //    };

        //    produtos.Add(produto3);

        //    foreach (var x in produtos)
        //    {
        //        if (x.ProdutoID == id)
        //        {
        //            produtos.Remove(x);
        //            return Ok(produtos);
        //        }

        //    }
        //    return BadRequest(new { ErrorMessage = "O ID não existe no banco." });
        //}

        //[HttpGet("getDesconto/{total}/{desconto}")]
        ////[Authorize]
        //[AllowAnonymous]
        //public async Task<IActionResult> GetCalculoResultado(float total, float desconto)
        //{

        //    var result = total * (desconto/100);

        //    return Ok(result);
        //}
    }
}
