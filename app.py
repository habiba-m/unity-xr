from flask import Flask, request, redirect, render_template, jsonify, send_file
import os

app = Flask(__name__)

# Config for file uploads
app.config['UPLOAD_FOLDER'] = 'uploads/'
app.config['MAX_CONTENT_LENGTH'] = 100 * 1024 * 1024  # Max 100MB, ID requested this! 

ALLOWED_EXTENSIONS = {'csv'}

# Check if file is of allowed type
def allowed_file(filename):
    return '.' in filename and filename.rsplit('.', 1)[1].lower() in ALLOWED_EXTENSIONS

# Route for the home page with file upload
@app.route('/')
def index():
    return render_template('index.html')

# Route to handle file uploads
@app.route('/upload', methods=['POST'])
def upload_file():
    if 'files[]' not in request.files:
        return redirect('/')
    
    files = request.files.getlist('files[]')
    uploaded_files = []

    for file in files:
        if file and allowed_file(file.filename):
            # Save each file to the upload folder
            filename = file.filename
            file.save(os.path.join(app.config['UPLOAD_FOLDER'], filename))
            uploaded_files.append(filename)
    
    # Return a response with the list of successfully uploaded files
    return jsonify({
        "message": "Files successfully uploaded.",
        "files": uploaded_files
    })

# Sending Files to Unity
#@ap0p.route('/send-to-unity/<filename>', methods=['GET'])
#def send_to_unity(filename):
 #   """
  #  This endpoint will give the uploaded file to Unity.
   # """
    # Check if file exists in the upload directory
    #file_path = os.path.join(app.config['UPLOAD_FOLDER'], filename)
    #if os.path.exists(file_path):
     #   return send_file(file_path, as_attachment=True)
    #else:
     #   return jsonify({"error": "File not found"}), 404
@app.route('/list-files', methods=['GET'])
def list_files():
    """
    This endpoint returns the list of uploaded CSV files.
    """
    files = os.listdir(app.config['UPLOAD_FOLDER'])
    csv_files = [f for f in files if allowed_file(f)]
    
    return jsonify({
        "files": csv_files
    })

# Run the server
if __name__ == '__main__':
    # Create the uploads directory if it doesn't exist
    if not os.path.exists(app.config['UPLOAD_FOLDER']):
        os.makedirs(app.config['UPLOAD_FOLDER'])
    app.run(debug=True)

