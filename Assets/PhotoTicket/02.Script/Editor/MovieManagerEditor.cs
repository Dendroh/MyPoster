using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(MovieManager))]
public class MovieManagerEditor : Editor
{
    private ReorderableList list;

    private void OnEnable()
    {

        list = new ReorderableList(
                serializedObject,
                serializedObject.FindProperty("movieList"),
                true, true, true, true);


        list.drawElementCallback =
        (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            rect.y += 3;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            var width = 120;
            var height = EditorGUIUtility.singleLineHeight;
            var rightMargin = 10;
            var topMargin = 2;
            list.elementHeight = EditorGUIUtility.singleLineHeight * 6;
            EditorGUI.LabelField(new Rect(rect.x, rect.y + ((height + topMargin) * 0), width, height), index + "번째 영화제목");
            EditorGUI.LabelField(new Rect(rect.x, rect.y + ((height + topMargin) * 1), width, height), "포스터 JPG(0.7:1)");
            EditorGUI.LabelField(new Rect(rect.x, rect.y + ((height + topMargin) * 2), width, height), "무료인가요?");
            EditorGUI.LabelField(new Rect(rect.x, rect.y + ((height + topMargin) * 3), width, height), "스티커");


            EditorGUI.PropertyField(
                        new Rect(rect.x + width + rightMargin, rect.y + ((height + topMargin) * 0), rect.width - width - rightMargin, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("MovieTitle"), GUIContent.none);

            EditorGUI.ObjectField(
                        new Rect(rect.x + width + rightMargin, rect.y + ((height + topMargin) * 1), rect.width - width - rightMargin, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("MoviePoster"), GUIContent.none);

            EditorGUI.PropertyField(
                        new Rect(rect.x + width + rightMargin, rect.y + ((height + topMargin) * 2), rect.width, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("Free"), GUIContent.none);

            EditorGUI.PropertyField(
                        new Rect(rect.x + width + rightMargin, rect.y + ((height + topMargin) * 3), rect.width, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("MovieSticker"));

            EditorGUILayout.PropertyField(element.FindPropertyRelative("MovieSticker"), true); // True means show children

        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("· 영화정보를 입력하세요. 순서대로 정렬되고, '+/-'로 추가/제거, '='으로 순서를 정렬할 수 있습니다\n" +
                                                        "· 포스터 JPG는 Texture Type = Sprite 여야 드래그가 가능합니다. 포스터이미지를 클릭해 바꾸세요.\n" +
                                                        "· 아래 Movie Sticker에 원하는 스티커를 올릴 수 있으며, 하위 5개는 각각 일종의 anchor 라고 생각하시면 됩니다.\n" +
                                                        "· 필요한 갯수만큼 Size를 입력하시고, 제작한 리소스를 Project창에서 드래그하여 올리면 됩니다.\n" +
                                                        "· 단, 리소스를 올리기 위해서는 유니티에서 지원하는 Prefab 형식이여야 하며, 만드는 방법은 어렵지 않습니다\n" +
                                                        "· 리소스 제작 시에는 Hierarch창->World->Flow Controller->No Touch Time 을 큰 수 (30000)로 설정하는게 편해요.\n" +
                                                        "  대신 설정 후에 꼭 No Touch Time을 다시 30으로 맞춰주셔야 합니다.\n\n" +
                                                        "       1. 이미지(Texture Type = Sprite) or obj 등을 Project창에서 Hierarchy창의 빈 곳으로 옮긴다.\n" +
                                                        "       2. 생성된 오브젝트를 다시 Hierarchy창에서 Project 창으로 옮긴다.\n" +
                                                        "       3. 파란 박스모양 아이콘이 보이면 Prefab이 만들어진 겁니다.\n" +
                                                        "       4. Hierarchy 창에 방금 만든 오브젝트(파란글씨)를 지운다.\n" +
                                                        "       5. 제작한 Prefab을 원하는 anchor에 할당한 뒤, 재생버튼을 눌러 인식되는지 확인한다.\n" +
                                                        "       6. 재생 상태에서 Hierichy -> Trackable 에서 방금 생성한 Clone을 찾아가 Transform값을 조정한다.\n" +
                                                        "       7. 잘 조절한 뒤 Transform을 우클릭->Copy Component 선택.\n" +
                                                        "       8. Project창의 해당 Prefab의 Transform을 우클릭->Paste Component Values 선택.\n" +
                                                        "       9. 정지\n\n" +
                                                        "       10. 모든 스티커에 대해서 적용한다.\n\n" +
                                                        "· 잘 안된다면 sg.ju@alcherainc.com으로 메일주세요.", MessageType.Info);
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}

/*
영화를 추가하거나 삭제할 때 아래에 있는 serialized property에서 할 수 있습니다.

<Serialized Property에 관한 설명>
1. 영화를 추가하거나 삭제하고 싶으면 맨 아래의 +, - 버튼을 눌러 추가, 삭제할 수 있습니다.
2. 영화를 추가하면 "(1)영화제목, (2)포스터 JPG, (3)무료인가요? (4)스티커" 네 가지 목록이 만들어집니다. 아래는 이 네 가지 목록들에 대한 설명입니다.
3. (1)영화제목: 이 부분에는 화면에 보여질 영화 제목을 적습니다. 
    (2)포스터 JPG(0.7:1): 이 부분에는 화면에 보여질 영화 포스터 이미지를 집어넣습니다. 'project' 창에 있는 영화 포스터 이미지를 이 부분에 드래그&드롭 해주면
                                    쉽게 포스터 이미지를 집어넣을 수 있습니다.
    
                                   **만약 포스터가 안 들어간다면?**
                                   포스터 이미지 타입이 맞지 않아서 그럴 수 있습니다. 이럴 때는, 포스터 이미지의 간단한 설정을 바꿔줌으로써 해결할 수 있습니다.
                                    1.먼저, 'project' 창에 있는 포스터 이미지를 클릭하고, 클릭하면 나오는 'inspector'창을 확인합니다.
                                    2.'inspector'창에서 포스터 이미지의 설정을 바꿔 줄 수 있습니다.
                                    3.가장 처음에 있는 'texture type' 을 클릭해서 <sprite(2D and UI)> 로 바꾸어 줍니다.
                                    4.바꾸었으면 맨 아래 오른쪽에 있는 'Revert'와 'Apply' 중 'Apply'를 클릭합니다. 이제 이미지 타입이 'sprite'가 되어, 이미지를 집어넣을 수 있게 변했습니다.
 
    (3)무료인가요?: 는 체크박스 형식입니다. 무료 타입인 영화는 체크박스를 클릭하고, 유료 타입인 영화는 체크박스를 해제해 주세요.
    (4)스티커: 사진을 찍을 때 얼굴에 붙일 스티커를 여기에서 정해 줄 수 있습니다. 스티커 옆의 '▼Movie Sticker' 를 클릭하세요.
                    클릭한 후에 드롭다운 메뉴를 찾으려면 가장 아래로 내려갑니다. 똑같은  '▼Movie Sticker' 목록이 여러 개가 있고,
                    클릭한 영화의 순서와 같은  '▼Movie Sticker' 목록이 켜지게 됩니다.
                    목록 아래에 다섯 개의 서브목록들(face centers, nose tips, mouths, left eyes, right eyes)가 보입니다. 이 서브목록들에 스티커를 추가해 주면 됩니다.
                    예를 들어, 코 쪽에 스티커를 붙이고 싶다(ex.돼지코?) 하면 'nose tips' 서브목록을 클릭합니다. 'size'에 추가할 스티커 갯수를 입력하면, 갯수만큼 아래에 목록이 생깁니다.
                    위에서 영화 포스터를 집어넣었던 것처럼, 제작한 스티커 리소스 파일을 이 목록에 드래그&드롭해주면 됩니다. 

                            **리스스를 올리기 전에 해주어야 할 작업**
                            스티커 리소스를 올리려면, 리소스 파일의 타입을 유니티가 지원하는 'prefab'이라는 타입으로 맞춰주어야 합니다. 아래 방법을 따라하시면 됩니다.
                                1.  'Project'창에 들어있는 스티커 리소스 파일을 끌어다 'Hierarchy' 창의 빈 곳에 놓는다.
                                2. 다시 'Hierarchy'창에 있는 스티커 리소스 파일을 끌어다 'Project' 창 안에 놓는다.
                                3. 파란 박스모양 아이콘을 가진 아이가 생성되면, 'Prefab' 타입이 만들어진 겁니다.
                                4. 만들어진 걸 확인했으므로, 'Hierarchy' 창에 방금 만든 오브젝트(파란글씨)를 지운다. (click and delete)
                    
                    이제, 'prefab'타입의 스티커 리소스 파일이 생겼으므로, 이 파일을 아까 원하는 서브목록들(face centers, nose tips, mouths...)에 집어넣어 주면 됩니다.
                    코 위치에 스티커를 붙여 주었지만, 정작 재생해보면 스티커 위치가 정확하지 않고 약간 어긋날 수도 있습니다.
                    스티커 모양들이 제각각 달라 코 중심에 딱 맞지 않을 수가 있기 때문에 그래요. 아래는 스티커 위치를 원하는 위치에 딱 맞추어 주는 작업에 관한 설명입니다.

                            **스티커 위치를 원하는 위치에 딱 맞추기**
                                1. 재생 상태에서 'Hierarchy'창에 있는 '▼trackable'을 찾습니다.
                                2. '▼trackable' 을 눌러, 추가한 스티커 목록을 클릭합니다. 코 스티커를 추가했다면, '▼NoseTip'을 찾아 클릭합니다.
                                3. '스티커  이름(Clone)'이라는 아이를 클릭한 뒤, 켜지는 'inspector' 창의 가장 위쪽에 'transform'-'position' 값을 조정해 줍니다.
                                4. 정확한 위치에 딱 맞추었다고 생각되면, 'transform' 제목 글씨를 우클릭 -> 나오는 메뉴에서 'Copy Component' 메뉴를 선택합니다.
                                5. 이제 커스텀해준 위치 값이 복사되었습니다. 원래 스티커 리소스에 붙여넣어 주어야 앞으로도 이 위치에 스티커가 나타납니다.
                                6. 'Project' 창에서, 위에서 만들어 주었던 해당하는 스티커의 'Prefab' 버전 파일을 찾습니다.
                                7. 'Prefab'파일을 클릭한 뒤, 나오는 'inspector' 창에서 'Transform' 제목을 우클릭 -> 나오는 메뉴에서 'Paste Component Values' 를 선택합니다.
                                8. 값이 복사되었습니다. 이제 Playmode(재생 모드)를 정지합니다.
                                9. 값을 조정해주어야 하는 스티커 리소스들에 대하여 이 작업을 반복하여 적용해 줍니다.

4. 이렇게 Playmode(재생) 상태에서. 스티커 리소스 파일 위치를 수정하는 작업을 할 때에는 먼저
    'Hierarchy' 창 -> 'World' -> 'Flow Controller' -> 'No Touch Time' 을 충분히 큰 수 (ex. 30000)로 설정하는게 편해요.
    키오스크 기능이 30초 동안 아무런 화면 터치가 이루어지지 않으면 초기 화면으로 돌아가게 구현되어 있기 때문에,
    화면 클릭 없이 30초가 넘으면 초기화면으로 돌아가 버려서 귀찮게 다시 작업 화면으로 들어가야 하는 불상사가 발생합니다.
    이를 방지하기 위해, 리소스 작업 때에는 충분히 큰 수로 초를 설정해 주는 것입니다. 
        **대신 스티커 리소스 작업이 끝나면 꼭 'No Touch Time'을 다시 30으로 맞춰주셔야 합니다.**






*/
