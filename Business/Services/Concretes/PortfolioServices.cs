using Business.Exceptions;
using Business.Services.Abstracts;
using Core.Models;
using Core.RepositoryAbstracts;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Concretes
{
    public class PortfolioServices : IPortfolioServices
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public PortfolioServices(IPortfolioRepository portfolioRepository, IWebHostEnvironment webHostEnvironment)
        {
            _portfolioRepository = portfolioRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public void AddPortfolio(Portfolio portfolio)
        {
            if (portfolio == null)
                throw new EntityNullException("Entity not found");
            if(portfolio.PhotoFile==null)
                throw new EntityNullException("Entity not found");
            if (!portfolio.PhotoFile.ContentType.Contains("image/"))
                throw new ContentTypeException("PhotoFile", "Content type error");
            if(portfolio.PhotoFile.Length>3000000)
                throw new FileSizeException("PhotoFile", "Content type error");

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(portfolio.PhotoFile.FileName);
            string path = _webHostEnvironment.WebRootPath + @"\Uploads\Portfolio\" + fileName;
            using(FileStream stream=new FileStream(path, FileMode.Create))
            {
                portfolio.PhotoFile.CopyTo(stream);
            }


            portfolio.ImgUrl = fileName;
            _portfolioRepository.Add(portfolio);
            _portfolioRepository.Commit();

        }

        public void DeletePortfolio(int id)
        {
            var portfolio=_portfolioRepository.Get(x=>x.Id == id);
            if(portfolio == null)
                throw new EntityNullException("Entity not found");
           
            string path = _webHostEnvironment.WebRootPath + @"\Uploads\Portfolio\" + portfolio.ImgUrl;
            if(!File.Exists(path))
                throw new Exceptions.FileNotFoundException("PhotoFile", "Content type error");
            File.Delete(path);

            _portfolioRepository.Delete(portfolio);
            _portfolioRepository.Commit();

        }

        public List<Portfolio> GetAllPortfolio(Func<Portfolio, bool>? func = null)
        {
            return _portfolioRepository.GetAll(func);
        }

        public Portfolio GetPortfolio(Func<Portfolio, bool>? func = null)
        {
            return _portfolioRepository.Get(func);
        }

        public void UpdatePortfolio(int id, Portfolio portfolio)
        {
            var oldportfolio=_portfolioRepository.Get(x=>x.Id==id);    
            if(oldportfolio == null)
                throw new EntityNullException("Entity not found");
            if (portfolio.PhotoFile != null)
            {
                if(!portfolio.PhotoFile.ContentType.Contains("image/"))  
                    throw new ContentTypeException("PhotoFile", "Content type error");

                if(portfolio.PhotoFile.Length>3000000)
                    throw new FileSizeException("PhotoFile", "Content type error");

                string path = _webHostEnvironment.WebRootPath + @"\Uploads\Portfolio\" + oldportfolio.ImgUrl;
                if(!File.Exists(path))
                    throw new Exceptions.FileNotFoundException("PhotoFile", "Content type error");
                File.Delete(path);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(portfolio.PhotoFile.FileName);
                string path1 = _webHostEnvironment.WebRootPath + @"\Uploads\Portfolio\" + fileName;
                using (FileStream stream = new FileStream(path1, FileMode.Create))
                {
                    portfolio.PhotoFile.CopyTo(stream);
                }

                oldportfolio.ImgUrl=fileName; 
            }
            oldportfolio.Name=portfolio.Name;
            oldportfolio.Description=portfolio.Description;
            _portfolioRepository.Commit();

        }
    }
}
