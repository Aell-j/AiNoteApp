
function show_results(p_id, result) {
    const element = document.getElementById(p_id);
    element.textContent = `Статус ${result.status} ${result.ok}\n` +
        JSON.stringify(result.data, null, 2);
}

function val(id) {
    return document.getElementById(id).value;
}

// USER
async function login_run()
{
    const result = await Login(val("login_user"), val("password_user"));
    show_results('result_login', result);
}

async function register_run()
{
    const result = await Register(val("email_register"), val('login_register'), val("password_register"));
    show_results('result_register', result);
}

async function get_all_users()
{
    const result =  await GetAllUser();
    show_results('result_getAllUsers', result);
}

async function get_user_by_id()
{
    const result =  await GetById(val("user_id"));
    show_results('result_user_id', result);
}

async function get_user_by_email()
{
    const result =  await GetByEmail(val("user_email"));
    show_results('result_user_email', result);
}

async function get_user_by_login()
{
    const result =  await GetByLogin(val("user_login"));
    show_results('result_user_login', result);
}

async function update_user()
{
    const result = await UpdateUser(val("user_update_id"), val("user_update_email"), val("user_update_login"), val("user_update_password"));
    show_results('result_update_user', result);
}

async function delete_user()
{
    const result = await DeleteUser(val("user_delete_id"));
    show_results('result_delete_user', result);
}

// FOLDER

async function add_folder()
{
    const result = await AddFolder(val("add_title"), val('add_parent_folder'), val("add_owner_id"));
    show_results('result_add_folder', result);
}
async function get_all_folders()
{
    const result =  await GetAllFolders();
    show_results('result_getAllFoldres', result);
}

async function get_folder_by_id()
{
    const result =  await GetByIdFolder(val("folder_id"));
    show_results('result_folder_id', result);
}

async function get_folder_by_owner_id()
{
    const result =  await GetByOwnerId(val("owner_id"));
    show_results('result_folder_owner_id', result);
}

async function get_folder_by_owner_id_parent_id()
{
    const result =  await GetByOwnerIdParentId(val("get_owner_id"), val("get_parent_id"));
    show_results('result_folder_owner_parent_id', result);
}

async function update_folder()
{
    const result = await UpdateFolder(val("folder_update_id"), val("folder_update_title"), val("folder_update_parent"));
    show_results('result_update_folder', result);
}

async function delete_folder()
{
    const result = await DeleteFolder(val("folder_delete_id"));
    show_results('result_delete_folder', result);
}

async function delete_folder_by_owner_id()
{
    const result = await DeleteFolderByOwner(val("folder_delete_owner_id"));
    show_results('result_delete_folder_owner_id', result);
}

// NOTE

async function add_note()
{
    const result = await AddNote(val("add_note_title"), val('add_content'), val("add_note_owner_id"), val("add_note_folder_id"), val("add_note_type"));
    show_results('result_add_note', result);
}
async function get_all_notes()
{
    const result =  await GetAllNotes();
    show_results('result_getAllNotes', result);
}

async function get_note_by_id()
{
    const result =  await GetByIdNote(val("note_id"));
    show_results('result_note_id', result);
}

async function get_notes_by_owner_id()
{
    const result =  await GetNoteByOwnerId(val("note_owner_id"));
    show_results('result_notes_owner_id', result);
}

async function get_notes_by_folder_id()
{
    const result =  await GetNoteByFolderId(val("note_folder_id"));
    show_results('result_note_folder_id', result);
}

async function update_note()
{
    const result = await UpdateNote(val("note_update_id"), val("update_note_title"), val("update_note_title_content"), val("update_note_title_note_owner_id"),val("update_note_title_note_folder_id"), val("update_note_title_note_type"));
    show_results('result_update_note', result);
}

async function delete_note()
{
    const result = await DeleteNote(val("note_delete_id"));
    show_results('result_delete_note', result);
}

async function delete_note_by_owner_id()
{
    const result = await DeleteNoteByOwner(val("note_delete_owner_id"));
    show_results('result_delete_note_owner_id', result);
}

async function delete_note_by_folder_id()
{
    const result = await DeleteNoteByFolder(val("note_delete_folder_id"));
    show_results('result_delete_note_folder_id', result);
}

document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('btn_login').addEventListener('click', login_run);
    document.getElementById('btn_register').addEventListener('click', register_run);
    document.getElementById('btn_get_all_users').addEventListener('click', get_all_users);
    document.getElementById('btn_user_by_id').addEventListener('click', get_user_by_id);
    document.getElementById('btn_user_email').addEventListener('click', get_user_by_email);
    document.getElementById('btn_user_login').addEventListener('click', get_user_by_login);
    document.getElementById('btn_update_user').addEventListener('click', update_user);
    document.getElementById('btn_delete_user').addEventListener('click', delete_user);
    
    document.getElementById('btn_add_folder').addEventListener('click', add_folder);
    document.getElementById('btn_get_all_folders').addEventListener('click', get_all_folders);
    document.getElementById('btn_folder_by_id').addEventListener('click', get_folder_by_id);
    document.getElementById('btn_folder_owner_id').addEventListener('click', get_folder_by_owner_id);
    document.getElementById('btn_folder_owner_id_parent_id').addEventListener('click', get_folder_by_owner_id_parent_id);
    document.getElementById('btn_update_folder').addEventListener('click', update_folder);
    document.getElementById('btn_delete_folder').addEventListener('click', delete_folder);
    document.getElementById('btn_delete_folder_owner_id').addEventListener('click', delete_folder_by_owner_id);
    
    document.getElementById('btn_add_note').addEventListener('click', add_note);
    document.getElementById('btn_get_all_notes').addEventListener('click', get_all_notes);
    document.getElementById('btn_note_by_id').addEventListener('click', get_note_by_id);
    document.getElementById('btn_notes_owner_id').addEventListener('click', get_notes_by_owner_id);
    document.getElementById('btn_bote_folder_id').addEventListener('click', get_notes_by_folder_id);
    document.getElementById('btn_update_note').addEventListener('click',update_note);
    document.getElementById('btn_delete_note').addEventListener('click',delete_note);
    document.getElementById('btn_delete_note_owner_id').addEventListener('click', delete_note_by_owner_id);
    document.getElementById('btn_delete_note_folder_id').addEventListener('click', delete_note_by_folder_id);
});