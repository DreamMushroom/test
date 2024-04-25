using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorSclipt : MonoBehaviour
{
    //추가될 방 프리팹을 저장하는 배열
    public GameObject[] availableRooms;
    //현재 게임에 추가된 방 오브젝트 리스트
    public List<GameObject> currentRooms;

    //화면의 가로 크기(단위 : 유닛)
    float screenWidthPoints;
    //바닥 오브젝트 이름, const : 상수
    const string floor = "Floor";

    //추가될 오브젝트 프리팹을 저장하는 배열
    public GameObject[] availableObjects;
    // 현재 게임에 생성된 코인, 레이저 오브젝트 리스트
    public List<GameObject> objects;
    // 오브젝트간 최소 간격
    public float objectMinDistance = 5f;
    // 오브젝트간 최대 간격
    public float objectMaxDistance = 10f;
    // 오브젝트 y축 위치 최소값
    public float objectMinY = -0.8f;
    // 오브젝트 y축 위치 최대값
    public float objectMaxY = 0.8f;
    // 오브젝트 최소 회전값
    public float objectMinRotation = -40f;
    // 오브젝트 최대 회전값
    public float objectMaxRotation = 40f;

    bool fever = false;


    private void Start()
    {
        // 카메라의 사이즈 값을 2배로 곱해 세로 크기 계산
        float height = 2.0f * Camera.main.orthographicSize; //유니티 상에서 3.2로 설정되어있으므로 height : 6.4
        // 세로 크기를 구한 후 화면 비율을 곱해서 가로 크기를 계산
        screenWidthPoints = height * Camera.main.aspect;
    }
    private void Update()
    {
        GenerateRoomIfRequired();
        GenerateObjectsRequired();
        StartCoroutine(Fever());
    }

    private void AddRoom(float farthestRooomEndX)
    {
        // 방 프리팹들 중 하나를 랜덤으로 선택
        int randomRoomIndex = Random.Range(0, availableRooms.Length);
        // 선택된 방 오브젝트를 추가
        GameObject room = Instantiate(availableRooms[randomRoomIndex]);
        // 방의 가로 크기
        float roomwidth = room.transform.Find(floor).localScale.x;
        // 방의 중앙 위치
        float roomCenter = farthestRooomEndX + roomwidth / 2;
        // 구한 방의 위치로 오브젝트를 위치시킴
        room.transform.position = new Vector3(roomCenter, 0, 0);
        // 추가한 방을 현재 추가된 방 리스트에 추가
        currentRooms.Add(room);

    }

    private void GenerateRoomIfRequired()
    {
        // 삭제할 방의 목록을 저장하는 리스트
        List<GameObject> roomsToRemove = new List<GameObject>();
        // 지금 프레임에 방을 생성할지 여부
        bool addRooms = true;
        // 마우스 오브젝트의 x축 위치
        float playerX = transform.position.x;
        // 삭제할 방의 기준 위치를 구함
        float removeRoomX = playerX - screenWidthPoints;
        // 추가할 방의 기준 위치를 구함
        float addRoomX = playerX + screenWidthPoints;
        // 가장 오른쪽에 위치한 방의 오른쪽 끝 위치
        float farthestRoomEndX = 0f;

        // 현재 추가된 방을 하나씩 처리
        foreach (var room in currentRooms)
        {
            // room 오브젝트의 바닥오브젝트를 찾아 가로크기를 가져옴
            float roomWidth = room.transform.Find(floor).localScale.x;
            // 방의 중간위치에서 방의 크기의 절반을 뺀 왼쪽 끝 위치를 계산
            float roomStartX = room.transform.position.x - roomWidth / 2;
            // 방의 왼쪽 끝 위치에서 방의 크기를 더해 오른쪽 끝 위치를 계산

            //방의 왼쪽 끝 위치가 방 추가 기준 위치보다 오른쪽에 있으면 방 추가x
            float roomEndX = roomStartX + roomWidth;
            if (roomStartX > addRoomX)
                addRooms = false;
            //방 삭제 기준위치보다 왼쪽에 존재하는 방이 잇으면 방삭제 목록에 추가
            if (roomEndX < removeRoomX)
                roomsToRemove.Add(room);

            //가장 오른쪽 방의 오른쪽 끝 위치를 최대값 메소드를 이용하여 구함
            farthestRoomEndX = Mathf.Max(farthestRoomEndX, roomEndX);
                
        }

        //삭제할 방 오브젝트를 하나씩 접근하면서
        foreach (var room in roomsToRemove)
        {
            //리스트에서 제거
            currentRooms.Remove(room);
            //오브젝트 제거
            Destroy(room);
        }

        //방을 추가해야한다면 방을 추가
        if (addRooms)
            AddRoom(farthestRoomEndX);
    }

    private void AddObject(float lastObjectX)
    {
        // 추가할 오브젝트의 인덱스를 랜덤으로 구하기
        int randomIndex = 0;

        if (fever)
            randomIndex = Random.Range(0, availableObjects.Length - 1);
        else
            randomIndex = Random.Range(0, availableObjects.Length);

        // 랜덤으로 구한 인덱스 번호의 오브젝트를 생성
        GameObject obj = Instantiate(availableObjects[randomIndex]);
        // 새로운 오브젝트의 X축 위치를 계산
        float objectPositionX =
            lastObjectX + Random.Range(objectMinDistance, objectMaxDistance);
        // 새로운 오브젝트의 Y축 위치를 계산
        float randomY = Random.Range(objectMinY, objectMaxY);

        // 계산된 위치값을 오브젝트의 위치로 변경
        obj.transform.position = new Vector3(objectPositionX, randomY, 0);

        // 랜덤으로 회전값 계산
        float rotation = Random.Range(objectMinRotation, objectMaxRotation);

        // 랜덤 회전 값을 반영하여 쿼터니언으로 회전값 적용, 쿼터니언 : 유니티가 처리하는 좌표값
        obj.transform.rotation = Quaternion.Euler(Vector3.forward * rotation);
        // 오브젝트 리스트에 추가
        objects.Add(obj);
    }

    private void GenerateObjectsRequired()
    {
        // 플레이어의 X축 위치
        float playerX = transform.position.x;
        // 삭제할 오브젝트의 기준 위치
        float removeObjectX = playerX - screenWidthPoints;
        // 추가할 오브젝트의 기준 위치
        float addObjextX = playerX + screenWidthPoints;
        // 가장 오른쪽에 위치한 오브젝트의 x축 위치
        float farthestObjectX = 0;

        // 삭제할 오브젝트 리스트
        List<GameObject> objectsToRemove = new List<GameObject>();

        // 현재 추가되어 있는 오브젝트들을 하나씩 처리
        foreach (var obj in objects)
        {
            // 오브젝트의 X축 위치
            float objX = obj.transform.position.x;
            // 최대값 비교로 가장 오른쪽에 위치한 오브젝트의 위치를 저장
            farthestObjectX = Mathf.Max(farthestObjectX, objX);

            // 오브젝트 위치가 삭제 기준 위치보다 왼쪽이면 오브젝트 삭제 리스트에 추가
            if (objX < removeObjectX)
                objectsToRemove.Add(obj);
        }

        // 삭제 리스트에 추가된 오브젝트를 모두 제거
        foreach (var obj in objectsToRemove)
        {
            // 리스트에서 제거
            objects.Remove(obj);
            // 오브젝트 제거
            Destroy(obj);
        }

        // 가장 오른쪽에 위치한 오브젝트가 추가 기준 위치보다 작다면 오브젝트를 추가
        if (farthestObjectX < addObjextX)
            AddObject(farthestObjectX);
    }
    IEnumerator Fever()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f);
            fever = true;
            yield return new WaitForSeconds(10f);
            fever = false;
        }
    }
}
