<%@ Page %>
My View contents for UploadFile\Index.rails
<form action="ShowUploadFileName.rails" method="post" enctype="multipart/form-data" >
    <fieldset>
        <legend>File Choice</legend>
        <label>Choose a file</label>
        <input name="pic" type="file" />
    </fieldset>
    <input type="submit" value="Upload" />
</form>