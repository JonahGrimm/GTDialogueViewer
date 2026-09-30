use tauri_plugin_dialog::DialogExt;
use tauri::Emitter;
use std::fs;
use std::sync::Mutex;

struct StartupArgs(Mutex<Vec<String>>);

#[tauri::command]
fn get_startup_args(state: tauri::State<StartupArgs>) -> Vec<String> {
    state.0.lock().unwrap().clone()
}

#[tauri::command]
fn open_file_dialog(app: tauri::AppHandle) -> Option<String> {
    app.dialog()
        .file()
        .add_filter("GT Dialogue Data", &["gtdialogue"])
        .blocking_pick_file()
        .and_then(|p| p.as_path().map(|path| path.to_string_lossy().into_owned()))
}

#[tauri::command]
fn read_file_content(path: String) -> Result<String, String> {
    fs::read_to_string(&path).map_err(|e| e.to_string())
}

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    let initial_args: Vec<String> = std::env::args().collect();

    tauri::Builder::default()
        .manage(StartupArgs(Mutex::new(initial_args)))
        .plugin(tauri_plugin_dialog::init())
        .plugin(tauri_plugin_opener::init())
        .plugin(tauri_plugin_single_instance::init(|app, argv, _cwd| {
            // Fired when application is already running and a second instance tries to open
            let _ = app.emit("open-file-from-os", argv);
        }))
        .invoke_handler(tauri::generate_handler![
            greet,
            open_file_dialog,
            read_file_content,
            get_startup_args
        ])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}

#[tauri::command]
fn greet(name: &str) -> String {
    format!("Hello, {}! You've been greeted from Rust!", name)
}