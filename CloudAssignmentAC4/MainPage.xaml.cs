using System.Collections.ObjectModel;
using System.ComponentModel;
using CloudAssignmentAC4.Models;
using Firebase.Storage;

namespace CloudAssignmentAC4;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
	private FirebaseStorage firebaseStorage;
    private ImageModel selectedImage;
    private string uploadProgress;

    public ObservableCollection<ImageModel> UploadedImages { get; } = new ObservableCollection<ImageModel>();


    public event PropertyChangedEventHandler PropertyChanged;
    public ImageModel SelectedImage
    {
        get { return selectedImage; }
        set
        {
            selectedImage = value;
            OnPropertyChanged(nameof(SelectedImage));
        }
    }
    public string UploadProgress
    {
        get { return uploadProgress; }
        set
        {
            uploadProgress = value;
            OnPropertyChanged(nameof(UploadProgress));
        }
    }
    

    public MainPage()
	{
		InitializeComponent();
		
    
        firebaseStorage = new FirebaseStorage("alumeet-5fcc9.appspot.com");
        BindingContext = this;

    }


	private async void OnSelectFileClicked(object sender, EventArgs e) {
        FileResult result = await FilePicker.PickAsync();
        if (result != null)
        {
            using (var stream = await result.OpenReadAsync())
            {
                var imageData = new byte[stream.Length];
                await stream.ReadAsync(imageData.AsMemory( 0, (int)stream.Length));

                SelectedImage = new ImageModel
                {
                    FileName = result.FileName,
                    Data = imageData,
                    ContentType = result.ContentType
                };

                await UploadImageToFirebaseStorage(SelectedImage);
               
            }
        }
    }


    private async Task UploadImageToFirebaseStorage(ImageModel imageModel)
    {
        if (imageModel != null)
        {
            var firebaseStoragePath = $"images/{imageModel.FileName}";

            try
            {
                // Upload the image file directly to Firebase Storage
                var uploadTask = firebaseStorage.Child(firebaseStoragePath).PutAsync(new MemoryStream(imageModel.Data), CancellationToken.None);

                // Register an event handler to monitor the upload progress
                uploadTask.Progress.ProgressChanged += (s, e) =>
                {
                    // You can update a progress bar or display the progress percentage here
                    UploadProgress = $"Upload progress: {e.Percentage}%";
                };

                // Wait for the upload to complete
                var uploadSnapshot = await uploadTask;

                imageModel.DownloadUrl = await firebaseStorage.Child(firebaseStoragePath).GetDownloadUrlAsync();
            }
            catch (FirebaseStorageException ex)
            {
                // Handle Firebase Storage exception
                Console.WriteLine($"Firebase Storage Exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }






    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}


