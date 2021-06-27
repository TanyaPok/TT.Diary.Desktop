using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;
using TT.Diary.Desktop.ViewModels.Interlayer;

namespace TT.Diary.Desktop.ViewModels.Commands.SaveCommands
{
    public class SaveCommand<T> : RelayCommand<T>, IAttributedCommand where T : IStorable
    {
        public string Name => "Save";
        public string ImgUrl => "pack://application:,,,/Images/Toolbar/save.png";

        public SaveCommand(bool keepTargetAlive = false)
            : base
            (
                async (T storable) => await Save(storable),
                (T storable) => storable != null && storable.CanAcceptChanges(),
                keepTargetAlive
            )
        {
        }

        public async Task ExecuteAsync(object parameter)
        {
            await Save((T) parameter);
        }

        private static async Task Save(T storable)
        {
            if (storable.Id == default)
            {
                storable.Id = await Endpoint.CreateAsync(storable.GetCreateRequest());
            }
            else
            {
                await Endpoint.UpdateAsync(storable.GetUpdateRequest());
            }

            storable.AfterAcceptChanges();
        }
    }

    public class SaveCommand<T, TP> : RelayCommand<T>, IAttributedCommand where T : IStorable where TP : IStoreKeeper<T>
    {
        public string Name { get; }
        public string ImgUrl { get; }

        public SaveCommand(TP storeKeeper, bool keepTargetAlive = false, string name = "Save",
            string imgUrl = "pack://application:,,,/Images/Toolbar/save.png")
            : base
            (
                async (T storable) =>
                {
                    storeKeeper.BeforeAcceptChanges(storable);

                    if (storable.Id == default)
                    {
                        storable.Id = await Endpoint.CreateAsync(storable.GetCreateRequest());
                    }
                    else
                    {
                        await Endpoint.UpdateAsync(storable.GetUpdateRequest());
                    }
                    
                    storable.AfterAcceptChanges();
                    await storeKeeper.AfterAcceptChanges(storable);
                },
                storeKeeper.CanAcceptChanges,
                keepTargetAlive
            )
        {
            Name = name;
            ImgUrl = imgUrl;
        }
    }
}