using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RestMng.Core;
using RestMng.Domain;
using RestMng.Infrastructure;

namespace RestMng.API
{
    public class RstMngController : Controller
    {
        #region Private fields
        private readonly ILogger<RstMngController> _logger;
        private readonly IMapper _mapper;
        private readonly CustomersRepository _customersRepository;
        private readonly ClientsRepository _clientsRepository;
        private readonly InventoryRepository _inventoryRepository;
        private readonly MenuItemsRepository _menuItemsRepository;
        private readonly OrdersRepository _ordersRepository;
        private readonly OrderItemsRepository _orderItemsRepository;
        #endregion

        #region Constructor
        public RstMngController(ILogger<RstMngController> logger, IMapper mapper, CustomersRepository customersRepository, ClientsRepository clientsRepository, InventoryRepository inventoryRepository, MenuItemsRepository menuItemsRepository, OrdersRepository ordersRepository, OrderItemsRepository orderItemsRepository)    
        {
            _logger = logger;
            _mapper = mapper;
            _customersRepository = customersRepository;
            _clientsRepository = clientsRepository;
            _inventoryRepository = inventoryRepository;
            _menuItemsRepository = menuItemsRepository;
            _ordersRepository = ordersRepository;
            _orderItemsRepository = orderItemsRepository;
        }
        #endregion

        #region Customers
        [HttpGet]
        [Route("/customers")]
        [Route("/customers/{*id}")]
        public async Task<Object> GetCustomer(int? id)
        {
            if (id != null)
            {
                return _mapper.Map<Customer>(await _customersRepository.Get(id.Value));
            }
            else
            {
                return _mapper.Map<List<Customer>>(await _customersRepository.GetAll());
            }
        }
        [HttpPost("/customers/create")]
        public async Task<Object> CreateCustomer([FromBody] Customer customer)
        {
            return _mapper.Map<Customer>(await _customersRepository.Add(_mapper.Map<Customers>(customer)));
        }
        [HttpPut("/customers/update")]
        public async Task<Object> UpdateCustomer([FromBody] Customer customer)
        {
            var dbCst = await _customersRepository.Get(customer.CustomerID);
            if (dbCst != null)
            {
                dbCst.Name = customer.Name;
                dbCst.ContactInfo = customer.ContactInfo;
                return _mapper.Map<Customer>(await _customersRepository.Update(dbCst));
            }
            return NotFound();
        }
        [HttpDelete("/customers/delete/{id:int}")]
        public async Task<object> DeleteCustomer(int id)
        {
            if (await _customersRepository.Get(id) != null)
            { 
                return _mapper.Map<List<Customer>>(await _customersRepository.Delete(id)); 
            }
            return NotFound();
        }

        [HttpGet("/customersbill/{id:int}")]
        public async Task<IActionResult?> GetCustomerBill(int id)
        {
            var customer = await _customersRepository.Get(id);
            if (customer != null)
            {
                var order = await _ordersRepository.GetByKey(c => c.CustomerID == customer.CustomerID);
                var orderItems = await _orderItemsRepository.Get(o => o.OrderID == order.OrderID);
                order.OrderItems = orderItems;
                var menuItems = await _menuItemsRepository.Get(m => order.OrderItems.Select(o => o.ItemID).Contains(m.ItemID));

                order.Status = OrderStatus.Completed;
                await _ordersRepository.Update(order);

                return File(ReportsHelper.GenerateCheckPdf(customer, order, menuItems), "application/pdf", "bill.pdf");
            }
            return null;
        }

        #endregion

        #region Clients
        [HttpGet]
        [Route("/clients")]
        [Route("/clients/{*id}")]
        public async Task<Object> GetClient(int? id)
        {
            if (id != null)
            {
                return _mapper.Map<Client>(await _clientsRepository.Get(id.Value));
            }
            else
            {
                return _mapper.Map<List<Client>>(await _clientsRepository.GetAll());
            }
        }
        [HttpPost("/clients/create")]
        public async Task<Client> CreateClient([FromBody] Client client)
        {
            return _mapper.Map<Client>(await _clientsRepository.Add(_mapper.Map<Clients>(client)));
        }
        [HttpPut("/clients/update")]
        public async Task<Object> UpdateClient([FromBody] Client client)
        {
            var dbObj = await _clientsRepository.Get(client.ClientID);
            if (dbObj != null)
            {
                dbObj.Name = client.Name;
                dbObj.Role = client.Role;
                return _mapper.Map<Client>(await _clientsRepository.Update(dbObj));
            }
            return NotFound();
        }
        [HttpDelete("/clients/delete/{id:int}")]
        public async Task<Object> DeleteClient(int id)
        {
            if (await _clientsRepository.Get(id) != null)
            {
                return await _clientsRepository.Delete(id);
            }
            return NotFound();
        }
        [HttpGet]
        [Route("/report/clients/{*type}")]
        public async Task<IActionResult?> GetClients([FromRoute] string type)
        {
            var clients = await _clientsRepository.GetAll();
            var orders = await _ordersRepository.GetAll();
            var customers = await _customersRepository.GetAll();
            if (clients != null)
            {
                if (type == "excel")
                {
                    return File(ReportsHelper.ClientsGenerateExcel(clients, orders, customers), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "clients.xlsx");
                }
                return File(ReportsHelper.ClientsList(clients, orders, customers), "application/pdf", "clients.pdf");
            }
            return null;
        }
        #endregion

        #region Inventory
        [HttpGet]
        [Route("/inventory")]
        [Route("/inventory/{*id}")]
        public async Task<Object> GetInventory(int? id)
        {
            if (id != null)
            {
                return _mapper.Map<Storage>(await _inventoryRepository.Get(id.Value));
            }
            else
            {
                return _mapper.Map<List<Storage>>(await _inventoryRepository.GetAll());
            }
        }
        [HttpPost("/inventory/create")]
        public async Task<Object> CreateInventory([FromBody] Storage inventory)
        {
            if (await _menuItemsRepository.Get(inventory.ItemID) != null)
            {
                return _mapper.Map<Storage>(await _inventoryRepository.Add(_mapper.Map<Inventory>(inventory)));
            }
            return NotFound($"Menu item not found in database with id: {inventory.ItemID}");
        }
        [HttpPut("/inventory/update")]
        [HttpPut("/inventory/add")]
        public async Task<Object> UpdateInventory([FromBody] Storage inventory)
        {
            var dbObj = await _inventoryRepository.Get(inventory.ItemID);
            if (dbObj != null)
            {
                if (HttpContext.Request.Path.ToString().EndsWith("add"))
                {
                    dbObj.Quantity += inventory.Quantity;
                }
                else
                {
                    dbObj.Quantity = inventory.Quantity;
                }
                return _mapper.Map<Storage>(await _inventoryRepository.Update(dbObj));
            }
            return NotFound();
        }

        [HttpDelete("/inventory/delete/{id:int}")]
        public async Task<Object> DeleteInventory(int id)
        {
            if (await _inventoryRepository.Get(id) != null)
            {
                return await _inventoryRepository.Delete(id);
            }
            return NotFound();
        }

        [HttpGet]
        [Route("/report/products/{*type}")]
        public async Task<IActionResult?> GetProducts([FromRoute]string type)
        {
            var products = await _inventoryRepository.GetAll();
            var menuItems = await _menuItemsRepository.GetAll();
            if (products != null && menuItems != null)
            {
                if (type == "excel")
                {
                    return File(ReportsHelper.ProductsGenerateExcel(products, menuItems), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "inventory_products.xlsx");
                }
                return File(ReportsHelper.ProductList(products, menuItems), "application/pdf", "inventory_products.pdf");
            }
            return null;
        }
        #endregion

        #region Menu
        [HttpGet]
        [Route("/menu")]
        [Route("/menu/{*id}")]
        public async Task<Object> GetMenu(int? id)
        {
            if (id != null)
            {
                return _mapper.Map<MenuItem>(await _menuItemsRepository.Get(id.Value));
            }
            else
            {
                return _mapper.Map<List<MenuItem>>(await _menuItemsRepository.GetAll());
            }
        }
        [HttpPost("/menu/create")]
        public async Task<MenuItem> CreateMenu([FromBody] MenuItem menu)
        {
            return _mapper.Map<MenuItem>(await _menuItemsRepository.Add(_mapper.Map<MenuItems>(menu)));
        }
        [HttpPut("/menu/update")]
        public async Task<Object> UpdateMenu([FromBody] MenuItem menu)
        {
            var dbObj = await _menuItemsRepository.Get(menu.ItemID);
            if (dbObj != null)
            {
                dbObj.Price = menu.Price;
                dbObj.Description = menu.Description;
                dbObj.Name = menu.Name;
                return _mapper.Map<MenuItem>(await _menuItemsRepository.Update(dbObj));
            }
            return NotFound();
        }

        [HttpDelete("/menu/delete/{id:int}")]
        public async Task<Object> DeleteMenu(int id)
        {
            if (await _menuItemsRepository.Get(id) != null)
            {
                return await _menuItemsRepository.Delete(id);
            }
            return NotFound();
        }
        [HttpGet]
        [Route("/report/menu/{*type}")]
        public async Task<IActionResult?> GetMenu([FromRoute] string type)
        {
            var menuItems = await _menuItemsRepository.GetAll();
            if (menuItems != null)
            {
                if (type == "excel")
                {
                    return File(ReportsHelper.MenuGenerateExcel(menuItems), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "menu.xlsx");
                }
                return File(ReportsHelper.MenuList(menuItems), "application/pdf", "menu.pdf");
            }
            return null;
        }

        #endregion

        #region Order
        [HttpPost("/orders/create")]
        public async Task<Object> CreateOder([FromBody]Order order)
        {
            return _mapper.Map<Order>(await _ordersRepository.Add(_mapper.Map<Orders>(order)));
        }
        [HttpPost("/orderitems/create")]
        [HttpPut("/orderitems/update")]
        public async Task<Object> UpdateOrder([FromBody] List<OrderItem> orderItems)
        {
            return _mapper.Map<OrderItems>(await _orderItemsRepository.Set(_mapper.Map<OrderItems>(orderItems)));
        }
        #endregion
    }
}
