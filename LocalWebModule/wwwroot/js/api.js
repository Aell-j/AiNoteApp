const API = 'https://localhost:7080/api';
const API_USER = `${API}/auth`;
const API_NOTE = `${API}/notes`;
const API_FOLDER = `${API}/folders`;

async function call_api(url, options = {}){
    try {
        const response = await fetch(url, options);
        const text = await response.text();
        let data = null;
        if (text) {
            try {
                data = JSON.parse(text);
            } catch {
                data = text;
            }
        }
        return {ok: response.ok, status: response.status, data};
    }
    catch (error) {
        return {ok: false, status: 0, data: null, error: error.message};
    }
}

// USER

async function Login(login, password) {
   let data = await call_api(`${API_USER}/auth`, {
       method: 'POST', 
       headers: {"Content-Type": "application/json"}, 
       body: JSON.stringify({login, password})});
   return data;
}

async function Register(email, login, password) {
    let data = await call_api(`${API_USER}/register`, {
        method: 'POST', 
        headers: {"Content-Type": "application/json"}, 
        body: JSON.stringify({email, login, password})});
    return data;
}

async function GetAllUser() {
    let data = await call_api(`${API_USER}`, {method: 'GET', headers: {"Content-Type": "application/json"}});
    return data;
}

async function GetById(id) {
    let data = await call_api(`${API_USER}/${id}`, {method: 'GET', headers: {"Content-Type": "application/json"}});
    return data;
}

async function GetByEmail(email) {
    let data =  await call_api(`${API_USER}/${email}/email`, {method: 'GET', headers: {"Content-Type": "application/json"}});
    return data;
}

async function GetByLogin(login) {
    let data = await call_api(`${API_USER}/${login}/login`, {method: 'GET', headers: {"Content-Type": "application/json"}});
    return data;
}

async function UpdateUser(id, email, login, password)
{
    let data = await call_api(`${API_USER}/${id}`, {
        method: 'PUT', 
        headers: {"Content-Type": "application/json"},
        body: JSON.stringify({email, login, password})
    });
    return data;
}

async function DeleteUser(id) {
    let data = await call_api(`${API_USER}/${id}`, {method: 'DELETE', headers: {"Content-Type": "application/json"}});
    return data;
}

// FOLDER

async function AddFolder(title, parentId, ownerId) {
    let data = await call_api(`${API_FOLDER}`, {
        method: 'POST',
        headers: {"Content-Type": "application/json"},
        body: JSON.stringify({title, parentId, ownerId})});
    return data;
}

async function GetAllFolders() {
    let data = await call_api(`${API_FOLDER}`, {method: 'GET', headers: {"Content-Type": "application/json"}});
    return data;
}

async function GetByIdFolder(id) {
    let data = await call_api(`${API_FOLDER}/${id}`, {method: 'GET', headers: {"Content-Type": "application/json"}});
    return data;
}

async function GetByOwnerId(ownerId) {
    let data =  await call_api(`${API_FOLDER}/${ownerId}/owner`, {method: 'GET', headers: {"Content-Type": "application/json"}});
    return data;
}

async function GetByOwnerIdParentId(ownerId, parentId) {
    let data = await call_api(`${API_FOLDER}/${ownerId}/parent/${parentId}/owner`, {method: 'GET', headers: {"Content-Type": "application/json"}});
    return data;
}

async function UpdateFolder(id, title, parentId)
{
    let data = await call_api(`${API_FOLDER}/${id}`, {
        method: 'PUT',
        headers: {"Content-Type": "application/json"},
        body: JSON.stringify({title, parentId})
    });
    return data;
}

async function DeleteFolder(id) {
    let data = await call_api(`${API_FOLDER}/${id}`, {method: 'DELETE', headers: {"Content-Type": "application/json"}});
    return data;
}

async function DeleteFolderByOwner(ownerId) {
    let data = await call_api(`${API_FOLDER}/${ownerId}/owner/delete`, {method: 'DELETE', headers: {"Content-Type": "application/json"}});
    return data;
}

// NOTE

async function AddNote(title, content, ownerId, folderId, noteType) {
    let data = await call_api(`${API_NOTE}`, {
        method: 'POST',
        headers: {"Content-Type": "application/json"},
        body: JSON.stringify({title, content, ownerId, folderId, noteType})});
    return data;
}

async function GetAllNotes() {
    let data = await call_api(`${API_NOTE}`, {method: 'GET', headers: {"Content-Type": "application/json"}});
    return data;
}

async function GetByIdNote(id) {
    let data = await call_api(`${API_NOTE}/${id}`, {method: 'GET', headers: {"Content-Type": "application/json"}});
    return data;
}

async function GetNoteByOwnerId(ownerId) {
    let data =  await call_api(`${API_NOTE}/${ownerId}/owner`, {method: 'GET', headers: {"Content-Type": "application/json"}});
    return data;
}

async function GetNoteByFolderId(folderId) {
    let data = await call_api(`${API_NOTE}/${folderId}/folder`, {method: 'GET', headers: {"Content-Type": "application/json"}});
    return data;
}

async function UpdateNote(id, title, content, ownerId, folderId, noteType)
{
    let data = await call_api(`${API_NOTE}/${id}`, {
        method: 'PUT',
        headers: {"Content-Type": "application/json"},
        body: JSON.stringify({title, content, ownerId, folderId, noteType})
    });
    return data;
}

async function DeleteNote(id) {
    let data = await call_api(`${API_NOTE}/${id}`, {method: 'DELETE', headers: {"Content-Type": "application/json"}});
    return data;
}

async function DeleteNoteByOwner(ownerId) {
    let data = await call_api(`${API_NOTE}/${ownerId}/owner`, {method: 'DELETE', headers: {"Content-Type": "application/json"}});
    return data;
}

async function DeleteNoteByFolder(folderId) {
    let data = await call_api(`${API_NOTE}/${folderId}/folder`, {method: 'DELETE', headers: {"Content-Type": "application/json"}});
    return data;
}