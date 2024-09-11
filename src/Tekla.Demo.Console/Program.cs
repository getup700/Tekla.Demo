using System;
using Tekla.Demo.Console.Services;

namespace Tekla.Demo.Console
{
    public class Program
    {
        static void Main()
        {
            //var service = new BeamService();
            //var result = service.CreateBeam();
            //Console.WriteLine(result);

            var rebarService = new RebarService();
            rebarService.CreateHooping();
        }
    }
}
