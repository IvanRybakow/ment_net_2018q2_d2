using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Task_4.Models;

namespace Task_4.Controllers
{
    public class UserEntitiesController : ApiController
    {
        private readonly Task_4.Models.AppContext _db = new Task_4.Models.AppContext();

        // GET: api/UserEntities
        public async Task<IEnumerable<UserEntity>> GetUsers()
        {
            return await _db.Users.ToListAsync();
        }

        // GET: api/UserEntities/5
        [ResponseType(typeof(UserEntity))]
        public async Task<IHttpActionResult> GetUserEntity(int id)
        {
            UserEntity userEntity = await _db.Users.FindAsync(id);
            if (userEntity == null)
            {
                return NotFound();
            }

            return Ok(userEntity);
        }

        // PUT: api/UserEntities/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUserEntity(int id, UserEntity userEntity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != userEntity.Id)
            {
                return BadRequest();
            }

            _db.Entry(userEntity).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserEntityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/UserEntities
        [ResponseType(typeof(UserEntity))]
        public async Task<IHttpActionResult> PostUserEntity(UserEntity userEntity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Users.Add(userEntity);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = userEntity.Id }, userEntity);
        }

        // DELETE: api/UserEntities/5
        [ResponseType(typeof(UserEntity))]
        public async Task<IHttpActionResult> DeleteUserEntity(int id)
        {
            UserEntity userEntity = await _db.Users.FindAsync(id);
            if (userEntity == null)
            {
                return NotFound();
            }

            _db.Users.Remove(userEntity);
            await _db.SaveChangesAsync();

            return Ok(userEntity);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserEntityExists(int id)
        {
            return _db.Users.Any(e => e.Id == id);
        }
    }
}