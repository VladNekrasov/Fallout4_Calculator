using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using Fallout4_Calculator.Models;

namespace Fallout4_Calculator.Services
{
    public class MainDataBaseAsyncRepository
    {
        SQLiteAsyncConnection database;
        public MainDataBaseAsyncRepository(string databasePath)
        {
            database = new SQLiteAsyncConnection(databasePath);
        }
        public async Task CreateTables()
        {
            await database.CreateTableAsync<Objects>();
            await database.CreateTableAsync<Components>();
            await database.CreateTableAsync<Junk>();
            await database.CreateTableAsync<Objects_Components>();
            await database.CreateTableAsync<Junk_Components>();
         }

        public async Task<int> SaveObjectAsync(Objects objects)
        {
            await database.InsertAsync(objects);
            return objects.ID_0bject;
        }
        public async Task<IEnumerable<Objects>> GetObjectsAsync()
        {
            var result = await database.Table<Objects>().ToListAsync();
            return result;
        }
        public async Task<Objects> GetOneObjectsAsync(int id)
        {
            return await database.GetAsync<Objects>(id);
        }
        public async Task UpdateObjectAsync(Objects objects)
        {
            await database.UpdateAsync(objects);
        }
        public async Task DeleteObjectAsync(Objects objects)
        {
            await database.DeleteAsync(objects);
        }

        public async Task SaveComponentAsync(Components component)
        {
            var id = await database.InsertAsync(component);
        }
        public async Task<IEnumerable<Components>> GetComponentsAsync()

        {
            var result = await database.Table<Components>().ToListAsync();
            return result;
        }
        public async Task<Components> GetOneComponentsAsync(int id)
        {
            return await database.GetAsync<Components>(id);
        }
        public async Task UpdateComponentAsync(Components components)
        {
            await database.UpdateAsync(components);
        }
        public async Task DeleteComponentAsync(Components components)
        {
            await database.DeleteAsync(components);
        }


        public async Task<int> SaveJunkAsync(Junk junk)
        {
           await database.InsertAsync(junk);
           return junk.ID_Junk;
        }
        public async Task<IEnumerable<Junk>> GetJunkAsync()
        {
            var result = await database.Table<Junk>().ToListAsync();
            return result;
        }
        public async Task<Junk> GetOneJunkAsync(int id)
        {
            return await database.GetAsync<Junk>(id);
        }
        public async Task UpdateJunkAsync(Junk junk)
        {
            await database.UpdateAsync(junk);
        }
        public async Task DeleteJunkAsync(Junk junk)
        {
            await database.DeleteAsync(junk);
        }
        //Many-to-Many
        //Components_Object
        public Task<List<Objects_Components>> QueryObjectsComponentsAsync(int id_cs,int id_o)
        {
            string query = $"SELECT Objects_Components.ID_Object, Objects_Components.ID_Component, Objects_Components.Amount From Objects_Components  WHERE Objects_Components.ID_Object={id_o}  AND Objects_Components.ID_Component={id_cs}";
            return database.QueryAsync<Objects_Components>(query);
        }
        public async Task<int> SearchComponents_Objects_Async(int id_cs, int id_o)
        {
            var table = await QueryObjectsComponentsAsync(id_cs, id_o);
            return table.Count;
        }
        public async Task SaveComponents_Object_Async(int id_cs, int id_o,int am)
        {
            var new_rel = new Objects_Components
            {
                ID_Component = id_cs,
                ID_Object = id_o,
                Amount = am
            };
            int b = await App.Database.SearchComponents_Objects_Async(id_cs, id_o);
            if (b==0) await database.InsertAsync(new_rel);
        }
        public async Task<int> GetAmount_Components_Object_Async(int id_cs, int id_o)
        { 
            int b = await App.Database.SearchComponents_Objects_Async(id_cs, id_o);
            if (b==1)
            {
                string query = $"SELECT Objects_Components.Amount From Objects_Components  WHERE Objects_Components.ID_Object={id_o}  AND Objects_Components.ID_Component={id_cs}";
                foreach(var obj in await database.QueryAsync<Objects_Components>(query))
                {
                    return obj.Amount;
                }
            }
            return -1;
        }
        public async Task<List<Objects>> GetObjects_Components_Object(int id_cs)
        {
            List<Objects> list = new List<Objects>();
            string query = $"SELECT Objects_Components.ID_Object From Objects_Components WHERE Objects_Components.ID_Component={id_cs}";
            foreach (var obj in await database.QueryAsync<Objects_Components>(query))
            {
                list.Add(await App.Database.GetOneObjectsAsync(obj.ID_Object));
            }
            return list;
        }
        public async Task<List<Components>> GetComponents_Components_Object(int id_o)
        {
            List<Components> list = new List<Components>();
            string query = $"SELECT Objects_Components.ID_Component From Objects_Components WHERE Objects_Components.ID_Object={id_o}";
            foreach (var obj in await database.QueryAsync<Objects_Components>(query))
            {
                list.Add(await App.Database.GetOneComponentsAsync(obj.ID_Component));
            }
            return list;
        }
        public async Task DeleteComponents_Object_Async(int id_cs, int id_o)
        {
            string query = $"DELETE From Objects_Components  WHERE Objects_Components.ID_Object={id_o}  AND Objects_Components.ID_Component={id_cs}";
            await database.QueryAsync<Objects_Components>(query);
        }
        public async Task Delete_Components_Components_Object_Async(int id_cs)
        {
            string query = $"DELETE From Objects_Components  WHERE Objects_Components.ID_Component={id_cs}";
            await database.QueryAsync<Objects_Components>(query);
        }
        public async Task Delete_Object_Components_Object_Async(int id_o)
        {
            string query = $"DELETE From Objects_Components  WHERE Objects_Components.ID_Object={id_o}";
            await database.QueryAsync<Objects_Components>(query);
        }
        //Components_Junk
        public Task<List<Junk_Components>> QueryJunkComponentsAsync(int id_cs, int id_j)
        {
            string query = $"SELECT Junk_Components.ID_Junk, Junk_Components.ID_Component, Junk_Components.Amount From Junk_Components  WHERE Junk_Components.ID_Component={id_cs}  AND Junk_Components.ID_Junk={id_j}";
            return database.QueryAsync<Junk_Components>(query);
        }
        public async Task<int> SearchComponents_Junk_Async(int id_cs, int id_j)
        {
            var table = await QueryJunkComponentsAsync(id_cs, id_j);
            return table.Count;
        }
        public async Task SaveComponents_Junk_Async(int id_cs, int id_j, int am)
        {
            var new_rel = new Junk_Components
            {
                ID_Component = id_cs,
                ID_Junk = id_j,
                Amount = am
            };
            int b = await App.Database.SearchComponents_Junk_Async(id_cs, id_j);
            if (b == 0) await database.InsertAsync(new_rel);
        }
        public async Task<int> GetAmount_Components_Junk_Async(int id_cs, int id_j)
        {
            int b = await App.Database.SearchComponents_Junk_Async(id_cs, id_j);
            if (b == 1)
            {
                string query = $"SELECT Junk_Components.Amount From Junk_Components  WHERE Junk_Components.ID_Junk={id_j}  AND Junk_Components.ID_Component={id_cs}";
                foreach (var obj in await database.QueryAsync<Junk_Components>(query))
                {
                    return obj.Amount;
                }
            }
            return -1;
        }
        public async Task<List<Junk>> GetJunk_Components_Junk(int id_cs)
        {
            List<Junk> list = new List<Junk>();
            string query = $"SELECT Junk_Components.ID_Junk From Junk_Components WHERE Junk_Components.ID_Component={id_cs}";
            foreach (var obj in await database.QueryAsync<Junk_Components>(query))
            {
                list.Add(await App.Database.GetOneJunkAsync(obj.ID_Junk));
            }
            return list;
        }
        public async Task<List<Components>> GetComponents_Components_Junk(int id_j)
        {
            List<Components> list = new List<Components>();
            string query = $"SELECT Junk_Components.ID_Component From Junk_Components WHERE Junk_Components.ID_Junk={id_j}";
            foreach (var obj in await database.QueryAsync<Junk_Components>(query))
            {
                list.Add(await App.Database.GetOneComponentsAsync(obj.ID_Component));
            }
            return list;
        }
        public async Task DeleteComponents_Junk_Async(int id_cs, int id_j)
        {
            string query = $"DELETE From Junk_Components  WHERE Junk_Components.ID_Junk={id_j}  AND Junk_Components.ID_Component={id_cs}";
            await database.QueryAsync<Junk_Components>(query);
        }
        public async Task Delete_Components_Components_Junk_Async(int id_cs)
        {
            string query = $"DELETE From Junk_Components  WHERE Junk_Components.ID_Component={id_cs}";
            await database.QueryAsync<Junk_Components>(query);
        }
        public async Task Delete_Junk_Components_Junk_Async(int id_j)
        {
            string query = $"DELETE From Junk_Components  WHERE Junk_Components.ID_Junk={id_j}";
            await database.QueryAsync<Junk_Components>(query);
        }
        //Many-to-Many
    }
}
